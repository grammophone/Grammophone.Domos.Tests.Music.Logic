using System.Threading.Tasks;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Record label administration manager.
	/// </summary>
	public class RecordLabelsManager : MusicManager
	{
		public RecordLabelsManager(MusicSession session)
			: base(session)
		{
		}

		public async Task<RecordLabel> CreateAsync(string name)
		{
			var label = this.DomainContainer.RecordLabels.Create();
			label.Name = name;

			this.DomainContainer.RecordLabels.Add(label);
			await this.DomainContainer.SaveChangesAsync();

			return label;
		}
	}
}
