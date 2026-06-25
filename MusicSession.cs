using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grammophone.Domos.Logic;
using Grammophone.Domos.Tests.Music.DataAccess;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Logic session root for the music test application.
	/// </summary>
	public class MusicSession : LogicSession<MusicUser, IMusicDomosDomainContainer>
	{
		/// <summary>
		/// Create.
		/// </summary>
		public MusicSession(string configurationSectionName, Expression<Func<MusicUser, bool>> userPickPredicate)
			: base(configurationSectionName, userPickPredicate)
		{
		}

		/// <summary>
		/// User currently used for access decisions.
		/// </summary>
		public MusicUser ActingMusicUser => this.ActingUser;

		/// <summary>
		/// Get catalog manager.
		/// </summary>
		public CatalogManager GetCatalogManager() => GetManager(() => new CatalogManager(this));

		/// <summary>
		/// Get record label manager.
		/// </summary>
		public RecordLabelsManager GetRecordLabelsManager() => GetManager(() => new RecordLabelsManager(this));

		/// <summary>
		/// Get artists manager.
		/// </summary>
		public ArtistsManager GetArtistsManager(RecordLabel label) => GetManager(() => new ArtistsManager(this, label), label.ID);

		/// <summary>
		/// Get albums manager.
		/// </summary>
		public AlbumsManager GetAlbumsManager(RecordLabel label) => GetManager(() => new AlbumsManager(this, label), label.ID);

		/// <summary>
		/// Get tracks manager.
		/// </summary>
		public TracksManager GetTracksManager(RecordLabel label) => GetManager(() => new TracksManager(this, label), label.ID);

		/// <summary>
		/// Create an impersonation scope for tests.
		/// </summary>
		public Task<ImpersonationScope<MusicUser>> GetTestImpersonationScopeAsync(Expression<Func<MusicUser, bool>> userPickPredicate)
		{
			return GetImpersonationScopeAsync(userPickPredicate);
		}
	}
}
