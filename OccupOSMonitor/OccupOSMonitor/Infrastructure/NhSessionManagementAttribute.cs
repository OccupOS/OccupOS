// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NhSessionManagementAttribute.cs" company="UCL">
//    Open Source
// </copyright>
// <summary>
//   Defines the NhSessionManagementAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NHibernate;
using NHibernate.Context;

namespace OccupOSMonitor.Infrastructure {
    /// <summary>
    /// The nh session management attribute.
    /// </summary>
    public class NhSessionManagementAttribute : ActionFilterAttribute {
        /// <summary>
        /// Initializes a new instance of the <see cref="NhSessionManagementAttribute"/> class.
        /// </summary>
        public NhSessionManagementAttribute() {
            //SessionFactory = WebApiApplication.SessionFactory;
        }

        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        private ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// The on action executing.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            session.BeginTransaction();
        }

        /// <summary>
        /// The on action executed.
        /// </summary>
        /// <param name="actionExecutedContext">
        /// The action executed context.
        /// </param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            var session = SessionFactory.GetCurrentSession();
            var transaction = session.Transaction;
            if (transaction != null && transaction.IsActive) {
                transaction.Commit();
            }
            session = CurrentSessionContext.Unbind(SessionFactory);
            session.Close();
        }
    }
}