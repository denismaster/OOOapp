using System;

namespace OOOVote.Data.Entities
{
    public enum OrganizationRole
    {
        /// <summary>
        /// Участник
        /// </summary>
        Participant,

        /// <summary>
        /// Член наблюдательного совета
        /// </summary>
        Supervisor,

        /// <summary>
        /// Член коллективного исполнительного органа
        /// </summary>
        TopManager,

        /// <summary>
        /// Генеральный директор(Президент)
        /// </summary>
        CEO, 

        /// <summary>
        /// Ревизор
        /// </summary>
        Auditor,
    }
}
