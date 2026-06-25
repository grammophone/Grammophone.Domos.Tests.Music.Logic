using System.Linq;
using System.Threading.Tasks;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Artist manager scoped to a record label.
	/// </summary>
	public class ArtistsManager : MusicManager
	{
		public ArtistsManager(MusicSession session, RecordLabel recordLabel)
			: base(session, recordLabel)
		{
		}

		public IQueryable<Artist> Artists => this.DomainContainer.Artists.Where(a => a.RecordLabelID == this.RecordLabel.ID);

		public async Task<Artist> CreateAsync(string name)
		{
			var artist = new Artist();
			artist.Name = name;
			artist.RecordLabelID = this.RecordLabel.ID;
			artist.RecordLabel = this.RecordLabel;

			this.DomainContainer.Artists.Add(artist);
			await this.DomainContainer.SaveChangesAsync();

			return artist;
		}
	}
}
