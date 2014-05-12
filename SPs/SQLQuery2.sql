Declare @StartResult int = 0, @EndResult int = 0,
@Filter Int = 0, @SearchTerm varchar(255) = '', @SearchUserId int = 0,
@UserId int = 0

DECLARE @skipRows Int = 0 --Change to input parameter to sp
DECLARE @takeRows Int = 20 --Change to input parameter to sp

If Exists (Select * From tempdb..sysobjects Where id = object_id(N'[tempdb]..[#tmpStories]'))
	Drop Table [dbo].[#tmpStories]

Create Table #tmpStories(ID Int, UserId Int, Title nVarchar(max), Post nVarchar(max), Name nVarchar(max), CreateDate DateTime, Likes Int, Readings Int)

Insert Into [#tmpStories](ID, UserId, Title, Post, Name, CreateDate, Likes, Readings)
Select ID, UserId, Title, Post, Name, CreateDate, 0, 0
From UserPosts
Where IsNull(IsActive, 1) = 1
	And (@SearchUserId = 0 Or UserId = @SearchUserId)
	And Replace(IsNull (Title, '') + IsNull(Post, ''), ' ', '') Like '%' + Replace(@SearchTerm, ' ', '') + '%'

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

Select *
From (
    Select ROW_NUMBER() OVER (ORDER BY UserId) As RowNumber, *
	From [#tmpStories]
    ) As [t1]
Where [t1].RowNumber Between @skipRows + 1 AND @skipRows + @takeRows 

Drop Table [dbo].[#tmpStories]