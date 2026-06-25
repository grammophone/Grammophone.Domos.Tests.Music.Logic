using System;
using Grammophone.Domos.AccessChecking;
using Grammophone.Domos.AccessChecking.Configuration;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Programmatic permissions setup for the music test logic.
	/// </summary>
	public class MusicPermissionsSetupProvider : IPermissionsSetupProvider
	{
		/// <inheritdoc/>
		public PermissionsSetup Load()
		{
			var setup = new PermissionsSetup();

			setup.DefaultRolesForAuthenticated.Add(new Reference { CodeName = MusicRoleNames.Authenticated });

			setup.PermissionAssignments.Add(CreateAdministratorAssignment());
			setup.PermissionAssignments.Add(CreateAuthenticatedAssignment());
			setup.PermissionAssignments.Add(CreateCatalogReaderAssignment());
			setup.PermissionAssignments.Add(CreateRecordLabelAdministratorAssignment());
			setup.PermissionAssignments.Add(CreateContributorAssignment());

			return setup;
		}

		private static PermissionAssignment CreateAdministratorAssignment()
		{
			var permission = new Permission { CodeName = "Administrator" };

			foreach (var entityType in new[]
			{
				typeof(MusicUser), typeof(Role), typeof(DispositionType), typeof(RecordLabel), typeof(RecordLabelAdministrator), typeof(RecordLabelContributor),
				typeof(Artist), typeof(Album), typeof(Track), typeof(AlbumStateTransition), typeof(WorkflowGraph), typeof(StateGroup), typeof(State), typeof(StatePath)
			})
			{
				permission.EntityAccesses.Add(CreateEntityAccess(entityType, all: true));
			}

			AddManagers(permission, typeof(CatalogManager), typeof(RecordLabelsManager), typeof(ArtistsManager), typeof(AlbumsManager), typeof(TracksManager));
			AddPaths(permission, AlbumWorkflowNames.SubmitForReview, AlbumWorkflowNames.ApproveForRelease, AlbumWorkflowNames.RejectRelease, AlbumWorkflowNames.ArchiveAlbum);

			return AssignToRole(permission, MusicRoleNames.Administrator);
		}

		private static PermissionAssignment CreateCatalogReaderAssignment()
		{
			var permission = new Permission { CodeName = "CatalogReader" };

			foreach (var entityType in new[] { typeof(RecordLabel), typeof(Artist), typeof(Album), typeof(Track), typeof(State) })
			{
				permission.EntityAccesses.Add(new EntityAccess { EntityType = entityType, CanRead = true });
			}

			AddManagers(permission, typeof(CatalogManager));

			return AssignToRole(permission, MusicRoleNames.CatalogReader);
		}

		private static PermissionAssignment CreateAuthenticatedAssignment()
		{
			var permission = new Permission { CodeName = "AuthenticatedWorkflowMetadata" };

			foreach (var entityType in new[] { typeof(WorkflowGraph), typeof(StateGroup), typeof(State), typeof(StatePath), typeof(RecordLabel) })
			{
				permission.EntityAccesses.Add(new EntityAccess { EntityType = entityType, CanRead = true });
			}

			return AssignToRole(permission, MusicRoleNames.Authenticated);
		}

		private static PermissionAssignment CreateRecordLabelAdministratorAssignment()
		{
			var permission = new Permission { CodeName = "RecordLabelAdministration" };

			foreach (var entityType in new[] { typeof(RecordLabel), typeof(Artist), typeof(Album), typeof(Track), typeof(AlbumStateTransition), typeof(WorkflowGraph), typeof(StateGroup), typeof(State), typeof(StatePath) })
			{
				permission.EntityAccesses.Add(CreateEntityAccess(entityType, all: true));
			}

			AddManagers(permission, typeof(CatalogManager), typeof(ArtistsManager), typeof(AlbumsManager), typeof(TracksManager));
			AddPaths(permission, AlbumWorkflowNames.SubmitForReview, AlbumWorkflowNames.ApproveForRelease, AlbumWorkflowNames.RejectRelease, AlbumWorkflowNames.ArchiveAlbum);

			return AssignToDispositionType(permission, MusicDispositionTypeNames.RecordLabelAdministrator);
		}

		private static PermissionAssignment CreateContributorAssignment()
		{
			var permission = new Permission { CodeName = "RecordLabelContribution" };

			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(RecordLabel), CanRead = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(Artist), CanRead = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(Album), CanRead = true, CanCreateOwn = true, CanReadOwn = true, CanWriteOwn = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(Track), CanRead = true, CanCreateOwn = true, CanReadOwn = true, CanWriteOwn = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(AlbumStateTransition), CanRead = true, CanCreateOwn = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(WorkflowGraph), CanRead = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(StateGroup), CanRead = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(State), CanRead = true });
			permission.EntityAccesses.Add(new EntityAccess { EntityType = typeof(StatePath), CanRead = true });

			AddManagers(permission, typeof(CatalogManager), typeof(AlbumsManager), typeof(TracksManager));
			AddPaths(permission, AlbumWorkflowNames.SubmitForReview);

			return AssignToDispositionType(permission, MusicDispositionTypeNames.RecordLabelContributor);
		}

		private static EntityAccess CreateEntityAccess(Type entityType, bool all)
		{
			return new EntityAccess { EntityType = entityType, CanCreate = all, CanRead = all, CanWrite = all, CanDelete = all };
		}

		private static void AddManagers(Permission permission, params Type[] managerTypes)
		{
			foreach (var managerType in managerTypes)
			{
				permission.ManagerAccesses.Add(new ManagerAccess { ManagerType = managerType });
			}
		}

		private static void AddPaths(Permission permission, params string[] statePathCodeNames)
		{
			foreach (var statePathCodeName in statePathCodeNames)
			{
				permission.StatePathAccesses.Add(new StatePathAccess { StatePathCodeName = statePathCodeName });
			}
		}

		private static PermissionAssignment AssignToRole(Permission permission, string roleCodeName)
		{
			var assignment = new PermissionAssignment { Permission = permission };
			assignment.Roles.Add(new Reference { CodeName = roleCodeName });

			return assignment;
		}

		private static PermissionAssignment AssignToDispositionType(Permission permission, string dispositionTypeCodeName)
		{
			var assignment = new PermissionAssignment { Permission = permission };
			assignment.DispositionTypes.Add(new Reference { CodeName = dispositionTypeCodeName });

			return assignment;
		}
	}
}
