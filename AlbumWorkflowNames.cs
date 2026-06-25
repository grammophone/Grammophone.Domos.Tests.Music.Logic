namespace Grammophone.Domos.Tests.Music.Logic
{
	/// <summary>
	/// Workflow names for the album publishing workflow.
	/// </summary>
	public static class AlbumWorkflowNames
	{
		public const string Graph = "AlbumPublishing";
		public const string Draft = "Draft";
		public const string ReviewPending = "ReviewPending";
		public const string Published = "Published";
		public const string Rejected = "Rejected";
		public const string Archived = "Archived";
		public const string SubmitForReview = "SubmitForReview";
		public const string ApproveForRelease = "ApproveForRelease";
		public const string RejectRelease = "RejectRelease";
		public const string ArchiveAlbum = "ArchiveAlbum";
	}
}
