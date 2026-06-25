using System;
using Grammophone.Domos.Logic;
using Grammophone.Domos.Tests.Music.DataAccess;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Base class for music test managers.
	/// </summary>
	public abstract class MusicManager : Manager<MusicUser, IMusicDomosDomainContainer, MusicSession>
	{
		protected MusicManager(MusicSession session)
			: base(session)
		{
		}

		protected MusicManager(MusicSession session, RecordLabel recordLabel)
			: base(session)
		{
			this.RecordLabel = recordLabel ?? throw new ArgumentNullException(nameof(recordLabel));
		}

		/// <summary>
		/// Optional record label scope.
		/// </summary>
		public RecordLabel RecordLabel { get; }
	}
}
