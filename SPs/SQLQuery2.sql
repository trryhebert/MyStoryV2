SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec Stories_Get '', 0, 1, 1, 0, 1
ALTER PROCEDURE Stories_Get 
	@SearchTerm varchar(255) = '',
	@SearchUserId int = 0,
	@UserId int = 0,
	@Filter Int = 0,
	@skipRows Int = 0,
	@takeRows Int = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    If Exists (Select * From tempdb..sysobjects Where id = object_id(N'[tempdb]..[#tmpStories]'))
	Drop Table [dbo].[#tmpStories]

	Create Table #tmpStories(ID Int, UserId Int, Title nVarchar(max), Post nVarchar(max), Name nVarchar(max), CreateDate DateTime, Likes Int, Readings Int, IsFavorite bit, IsFollowed bit)

	Insert Into [#tmpStories](ID, UserId, Title, Post, Name, CreateDate, Likes, Readings, IsFavorite, IsFollowed)
	Select u.ID, u.UserId, u.Title, u.Post, up.UserName, u.CreateDate, 0, 0,
		Case When fa.ID Is Not Null Then 1 Else 0 End, Case When fw.ID Is Not Null Then 1 Else 0 End
	From UserPosts u
	Left Join UserProfile up on u.UserId = up.UserId
	Left Join UserFavorites fa on u.Id = fa.StoryId And fa.UserId = @UserId
	Left Join UserFollows fw on u.UserId = fw.FollowedUserId And fw.UserId = @UserId
	Where IsNull(u.IsActive, 1) = 1
		And (@SearchUserId = 0 Or u.UserId = @SearchUserId)
		And Replace(IsNull (u.Title, '') + IsNull(u.Post, ''), ' ', '') Like '%' + Replace(@SearchTerm, ' ', '') + '%'

	--Get count of likes and read.
	Update t1 Set t1.Likes = t2.Likes, t1.Readings = t2.Readings
	From #tmpStories t1 Inner Join
	(
		Select t.ID, Count(DISTINCT l.ID) As Likes, Count(Distinct r.ID) As Readings
		From #tmpStories t
		Left Join PostLikes l On t.ID = l.PostID
		Left Join PostReadings r On t.ID = r.PostID
		Group By t.ID
	) As t2 On	t1.ID = t2.ID

	--If filtered by favorites or followed
	Select Count(*)
		From [#tmpStories]
		Where IsFavorite = Case When @Filter = 3 Then 1 Else IsFavorite End
			  And IsFollowed = Case When @Filter = 4 Then 1 Else IsFollowed End

	Select *
	From (
		Select ROW_NUMBER() OVER (ORDER BY Case When @Filter = 0 Then CreateDate End Desc, Case When @Filter = 1 Then Likes End Desc, Case When @Filter = 2 Then Readings End Desc, CreateDate Desc) As RowNumber, *
		From [#tmpStories]
		Where IsFavorite = Case When @Filter = 3 Then 1 Else IsFavorite End
			  And IsFollowed = Case When @Filter = 4 Then 1 Else IsFollowed End
		) As [t1]
	Where [t1].RowNumber Between @skipRows + 1 AND @skipRows + @takeRows

	Drop Table [dbo].[#tmpStories]
END
GO
