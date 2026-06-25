using System.Linq;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Read-only catalog manager.
	/// </summary>
	public class CatalogManager : MusicManager
	{
		public CatalogManager(MusicSession session)
			: base(session)
		{
		}

		public IQueryable<RecordLabel> RecordLabels => this.DomainContainer.RecordLabels;

		public IQueryable<Artist> Artists => this.DomainContainer.Artists;

		public IQueryable<Album> Albums => this.DomainContainer.Albums;

		public IQueryable<Track> Tracks => this.DomainContainer.Tracks;
	}
}
