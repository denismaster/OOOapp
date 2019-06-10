using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace OOOVote.Pages
{
    public class VotingResult
    {
        public int AgreeCount { get; set; }
        public int DisagreeCount { get; set; }
        public int NotVotedCount { get; set; }

        public int AllVotedCount { get; set; }

    }

    public class VotingPdfResult
    {
        public string Question { get; set; }
        public int AgreeCount { get; set; }
        public int DisagreeCount { get; set; }
        public int NotVotedCount { get; set; }

        public int AllVotedCount { get; set; }

        public string Resolution { get; set; }

    }

    public class DetailsModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DetailsModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Voting Voting { get; set; }

        public Dictionary<Guid, VoteDecision> UserVotes { get; set; } = new Dictionary<Guid, VoteDecision>();

        public Dictionary<string, VotingResult> VotingResults { get; set; } = new Dictionary<string, VotingResult>();

        [BindProperty]
        public bool ShowResults { get; set; } = false;

        [BindProperty]
        public bool IsVotingEnded { get; set; } = false;

        [BindProperty]
        public int UserCount { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            Voting = await _context.Votings
                .Include(v => v.VotingMessages)
                    .ThenInclude(vm => vm.User)
                .Include(v => v.VotingOptions)
                    .ThenInclude(v => v.Decisions)
                .Include(v => v.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Voting == null)
            {
                return NotFound();
            }

            UserCount = (await _context.Organizations
                .Include(o => o.Users)
                .FirstOrDefaultAsync(o => o.Id == Voting.OrganizationId))
                ?.Users?.Count ?? 0;

            UserVotes = Voting.VotingOptions.ToDictionary(v => v.Id, v => VoteDecision.NotVoted);

            var userDecisions = Voting.VotingOptions
                .SelectMany(v => v.Decisions)
                .Where(d => d.UserId == userId)
                .ToList();

            bool hasUserDecisions = userDecisions.Count >= Voting.VotingOptions.Count;

            ShowResults = hasUserDecisions || Voting.EndDate < DateTime.Now;

            IsVotingEnded = Voting.EndDate < DateTime.Now || Voting.VotingOptions
                .Select(v => v.Decisions)
                .All(d => d.Count >= UserCount * 0.51);

            if (ShowResults)
            {
                VotingResults = Voting.VotingOptions
                    .Select(vo => new { Title = vo.Title, Decisions = vo.Decisions })
                    .ToDictionary(x => x.Title, vx =>
                      {
                          return new VotingResult
                          {
                              AgreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Agree),
                              DisagreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Disagree),
                              NotVotedCount = vx.Decisions.Count(x => x.Decision == VoteDecision.NotVoted),
                              AllVotedCount = vx.Decisions.Count()
                          };
                      });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadFile(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            Voting = await _context.Votings
                .Include(v => v.VotingMessages)
                    .ThenInclude(vm => vm.User)
                .Include(v => v.VotingOptions)
                    .ThenInclude(v => v.Decisions)
                .Include(v => v.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Voting == null)
            {
                return NotFound();
            }

            UserCount = (await _context.Organizations
                .Include(o => o.Users)
                .FirstOrDefaultAsync(o => o.Id == Voting.OrganizationId))
                ?.Users?.Count ?? 0;

            UserVotes = Voting.VotingOptions.ToDictionary(v => v.Id, v => VoteDecision.NotVoted);

            var userDecisions = Voting.VotingOptions
                .SelectMany(v => v.Decisions)
                .Where(d => d.UserId == userId)
                .ToList();

            bool hasUserDecisions = userDecisions.Count >= Voting.VotingOptions.Count;

            ShowResults = hasUserDecisions || Voting.EndDate < DateTime.Now;

            IsVotingEnded = Voting.EndDate < DateTime.Now || Voting.VotingOptions
                .Select(v => v.Decisions)
                .All(d => d.Count >= UserCount * 0.51);

            if (ShowResults)
            {
                VotingResults = Voting.VotingOptions
                    .Select(vo => new { Title = vo.Title, Decisions = vo.Decisions })
                    .ToDictionary(x => x.Title, vx =>
                    {
                        return new VotingResult
                        {
                            AgreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Agree),
                            DisagreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Disagree),
                            NotVotedCount = vx.Decisions.Count(x => x.Decision == VoteDecision.NotVoted),
                            AllVotedCount = vx.Decisions.Count()
                        };
                    });
            }

            return new FileStreamResult(CreateIListPdfReport(Voting.Subject, VotingResults, UserCount).PdfStreamOutput, "application/pdf");
        }

        public async Task<IActionResult> OnPostAsync(Guid? id, Dictionary<Guid, VoteDecision> userVotes)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            var decisions = userVotes.Select(pair => new UserVotingDecision
            {
                Decision = pair.Value,
                VotingOptionId = pair.Key,
                UserId = userId
            });

            _context.VotingDecisions.AddRange(decisions);

            await _context.SaveChangesAsync();

            return await OnGetAsync(id);
        }

        private IPdfReportData CreateIListPdfReport(string subject, Dictionary<string, VotingResult> votingResults, int userCount)
        {
            var stream = new MemoryStream();

            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata {
                    Author = "OOOVote",
                    Application = "OOOVote",
                    Subject = subject, Title = subject });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
                doc.PrintingPreferences(new PrintingPreferences
                {
                    ShowPrintDialogAutomatically = true
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message($"Голосование: {subject}");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.SimpleTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.NumberOfDataRowsPerPage(5);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(votingResults.Select(v => new VotingPdfResult
                {
                    Question = v.Key,
                    AgreeCount = v.Value.AgreeCount,
                    DisagreeCount = v.Value.DisagreeCount,
                    NotVotedCount = v.Value.NotVotedCount,
                    Resolution =
                    (v.Value.AgreeCount * 1.0 / userCount * 100 > 51)
                    ? "Принято решение - ЗА"
                    : (v.Value.DisagreeCount * 1.0 / userCount * 100 > 51)
                        ? "Принято решение - Против"
                        : "Решение не принято"
                }));
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNo");
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VotingPdfResult>(x => x.Question);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Вопрос");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VotingPdfResult>(x => x.AgreeCount);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Число голосов ЗА", horizontalAlignment: HorizontalAlignment.Left);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VotingPdfResult>(x => x.DisagreeCount);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Число голосов ПРОТИВ", horizontalAlignment: HorizontalAlignment.Left);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VotingPdfResult>(x => x.NotVotedCount);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Число ВОЗДЕРЖАВШИХСЯ", horizontalAlignment: HorizontalAlignment.Left);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VotingPdfResult>(x => x.Resolution);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Решение");
                    column.PaddingLeft(25);
                });

            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "Нет данных для отображения");
            })
            .Export(export =>
            {
                export.ToExcel();
                export.ToCsv();
                export.ToXml();
            })
            .Generate(data => data.AsPdfStream(stream,false));
        }
    }
}
