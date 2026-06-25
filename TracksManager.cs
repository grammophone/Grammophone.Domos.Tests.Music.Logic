using System.Linq;
using System.Threading.Tasks;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Track manager scoped to a record label.
	/// </summary>
	public class TracksManager : MusicManager
	{
		public TracksManager(MusicSession session, RecordLabel recordLabel)
			: base(session, recordLabel)
		{
		}

		public IQueryable<Track> Tracks => this.DomainContainer.Tracks.Where(t => t.RecordLabelID == this.RecordLabel.ID);

		public async Task<Track> CreateAsync(Album album, string title)
		{
			var track = new Track();
			track.Title = title;
			track.Album = album;
			track.AlbumID = album.ID;
			track.RecordLabel = this.RecordLabel;
			track.RecordLabelID = this.RecordLabel.ID;
			track.Owner = this.Session.ActingMusicUser;
			track.OwnerID = this.Session.ActingMusicUser.ID;

			this.DomainContainer.Tracks.Add(track);
			await this.DomainContainer.SaveChangesAsync();

			return track;
		}
	}
}
