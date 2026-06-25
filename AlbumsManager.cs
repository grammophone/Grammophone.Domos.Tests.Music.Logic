using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grammophone.DataAccess;
using Grammophone.DataAccess.QueryExtensions;
using Grammophone.Domos.Domain.Workflow;
using Grammophone.Domos.Logic;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Album manager scoped to a record label.
	/// </summary>
	public class AlbumsManager : MusicManager
	{
		public AlbumsManager(MusicSession session, RecordLabel recordLabel)
			: base(session, recordLabel)
		{
		}

		public IQueryable<Album> Albums => this.DomainContainer.Albums.Where(a => a.RecordLabelID == this.RecordLabel.ID);

		public async Task<Album> CreateDraftAsync(Artist artist, string title)
		{
			var draftState = await this.DomainContainer.States.SingleAsync(s => s.CodeName == AlbumWorkflowNames.Draft);
			var album = new Album();
			album.Title = title;
			album.Artist = artist;
			album.ArtistID = artist.ID;
			album.RecordLabel = this.RecordLabel;
			album.RecordLabelID = this.RecordLabel.ID;
			album.Owner = this.Session.ActingMusicUser;
			album.OwnerID = this.Session.ActingMusicUser.ID;
			album.State = draftState;
			album.StateID = draftState.ID;

			this.DomainContainer.Albums.Add(album);
			await this.DomainContainer.SaveChangesAsync();

			return album;
		}

		public Task<AlbumStateTransition> SubmitForReviewAsync(Album album)
			=> ExecutePathAsync(album, AlbumWorkflowNames.SubmitForReview);

		public Task<AlbumStateTransition> ApproveForReleaseAsync(Album album, DateTime releaseDate)
			=> ExecutePathAsync(album, AlbumWorkflowNames.ApproveForRelease, new Dictionary<string, object> { ["ReleaseDate"] = releaseDate });

		private async Task<AlbumStateTransition> ExecutePathAsync(Album album, string pathCodeName, IDictionary<string, object> arguments = null)
		{
			var statePath = await this.DomainContainer.StatePaths
				.Include(sp => sp.PreviousState)
				.Include(sp => sp.NextState)
				.Include(sp => sp.WorkflowGraph)
				.SingleAsync(sp => sp.CodeName == pathCodeName);

			if (!this.AccessResolver.CanUserExecuteStatePath(this.Session.ActingMusicUser, album, statePath))
			{
				throw new StatePathAccessDeniedException(statePath, album, $"Access to path '{pathCodeName}' is denied.");
			}

			using (var transaction = this.DomainContainer.BeginTransaction())
			using (GetElevatedAccessScope())
			{
				if (album.StateID != statePath.PreviousStateID)
				{
					throw new UserException($"Album '{album.Title}' is not in the expected state.");
				}

				var transition = new AlbumStateTransition();
				transition.Album = album;
				transition.AlbumID = album.ID;
				transition.Path = statePath;
				transition.PathID = statePath.ID;
				transition.ChangeStampBefore = album.ChangeStamp;

				album.State = statePath.NextState;
				album.StateID = statePath.NextStateID;
				album.LastStateChangeDate = DateTime.UtcNow;
				album.ChangeStamp &= statePath.ChangeStampANDMask;
				album.ChangeStamp |= statePath.ChangeStampORMask;
				transition.ChangeStampAfter = album.ChangeStamp;

				this.DomainContainer.StateTransitions.Add(transition);

				await transaction.CommitAsync();

				return transition;
			}
		}
	}
}
