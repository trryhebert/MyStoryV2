using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace shareurstorydb
{
    public class MyStoryContext : System.Data.Entity.DbContext
    {
        public MyStoryContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }
        public DbSet<PostShares> PostShares { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<PostReadings> PostReadings { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Logs> Logs { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ExternalUserInformation> ExternalUsers { get; set; }
        public DbSet<OAuthMembership> OAuthMemberships { get; set; }
    }

    //public class CreateDb : DropCreateDatabaseIfModelChanges<MyStoryContext>
    public class CreateDb : DropCreateDatabaseAlways<MyStoryContext>
    {
        protected override void Seed(MyStoryContext context)
        {
            context.UserProfiles.Add(new UserProfile
            {
                UserId = 1,
                UserName = "isabelle@test.com",
                isActive = true
            });
            context.UserProfiles.Add(new UserProfile
            {
                UserId = 2,
                UserName = "trryhebert@gmail.com",
                isActive = true
            });

            context.UserPosts.Add(new UserPost
            {
                CreateDate = DateTime.Now,
                Post = "<p>Emma Woodhouse is a beautiful, bright, well-off young woman. She is the youngest daughter of Mr. Woodhouse. Her older sister married years ago, and their mother died even longer ago--two events which left Emma as mistress of the house. The only real check on her rule was Miss Taylor, her kind and affectionate governess. For sixteen years Miss Taylor lived with the Woodhouse family, and she became a particular friend to Emma, in spirit more a sister. Under such kind care, Emma lived,</p>\"doing just what she liked; highly esteeming Miss Taylor''s judgments, but directed chiefly by her own. The real evils, indeed, of Emma''s situation were the power of having rather too much her own way, and a disposition to think a little too well of herself....\" Chapter 1, pg. 3</p><p>She was still a generally charming and kindhearted young woman, and her only recent sorrow was the loss of Miss Taylor. She had married the nice Mr. Weston, and she no longer lived with the Woodhouse family. Though it was a good match, and Emma had wanted this good fortune for her governess, she could not help but feel sadness. Emma had been Miss Taylor''s single charge since her sister Isabella married seven years ago, and it was hard for Emma to imagine spending her days without her. Though her new home was only a half mile from Hartfield, as Mrs. Weston she would not have the same time for Emma. And though Emma loved her father, he was not a playmate or a good conversationalist like Miss </p>",
                Title = "Jane Austin quote (emma)1",
                ID = 1,
                UserId = 2,
                UpdateDate = DateTime.Now,
                isActive = true
            });
            context.UserPosts.Add(new UserPost
            {
                CreateDate = DateTime.Now,
                Post = "<p>Mr. Weston was a native of Highbury, born into a family of the higher class. He was well educated and chose the military as his profession. He met a wealthy young woman, Miss Churchill, whom he married despite her family''s objections. Her family disowned her, and though she loved her husband, she missed Enscombe and her previous life of ease. She died after giving birth to a boy, Frank, whom Mr. Churchill and Mrs. Churchill took to raise.</p><p>After this Mr. Weston went into trade, and lived the next twenty years happy and alone in Highbury. His money belonged to himself, as his son would be the Churchill''s heir, not his own. Therefore he had the money to marry Miss Taylor, a governess with little dowry. Frank lived with the Churchill''s, though he was a great topic of conversation and curiosity in Highbury. The curiosity increased with the hope that Frank would visit his father and new stepmother. The town discussed it over tea, drooled over his well-written letters, and hoped for the day they could see this mysterious favored son. Mrs. Weston was happy and anxious to meet her new son, though she often regretted leaving Emma behind. But she felt comfort that Emma could take good care of herself.</p><p>After several weeks Mr. Woodhouse grew less depressed over the loss of Miss Taylor, though he often referred to her as \"poor Miss Taylor.\" Hating rich foods, Mr. Woodhouse''s stress lessened after the wedding cake was gone. He even went to the trouble of asking Mr. Perry, the doctor, about its'' ill effects; he discouraged everyone from eating it, and he seemed calmer once it was eaten and no longer a subject of discussion.</p>",
                Title = "Jane Austin quote (emma)2",
                ID = 2,
                UserId = 2,
                UpdateDate = DateTime.Now,
                isActive = true
            });
            context.UserPosts.Add(new UserPost
            {
                CreateDate = DateTime.Now,
                Post = "<p>WHEN we look to the individuals of the same variety or sub-variety of our older cultivated plants and animals, one of the first points which strikes us, is, that they generally differ much more from each other, than do the individuals of any one species or variety in a state of nature. When we reflect on the vast diversity of the plants and animals which have been cultivated, and which have varied during all ages under the most different climates and treatment, I think we are driven to conclude that this greater variability is simply due to our domestic productions having been raised under conditions of life not so uniform as, and somewhat different from, those to which the parent-species have been exposed under nature. There is, also, I think, some probability in the view propounded by Andrew Knight, that this variability may be partly connected with excess of food. It seems pretty clear that organic beings must be exposed during several generations to the new conditions of life to cause any appreciable amount of variation; and that when the organisation has once begun to vary, it generally continues to vary for many generations. No case is on record of a variable being ceasing to be variable under cultivation. Our oldest cultivated plants, such as wheat, still often yield new varieties: our oldest domesticated animals are still capable of rapid improvement or modification.</p><p>It has been disputed at what period of time the causes of variability, whatever they may be, generally act; whether during the early or late period of development of the embryo, or at the instant of conception. Geoffroy St Hilaire''s experiments show that unnatural treatment of the embryo causes monstrosities; and monstrosities cannot be separated by any clear line of distinction from mere variations. But I am strongly inclined to suspect that the most frequent cause of variability may be attributed to the male and female reproductive elements having been affected prior to the act of conception. Several reasons make me believe in this; but the chief one is the remarkable effect which confinement or cultivation has on the functions of the reproductive system; this system appearing to be far more susceptible than any other part of the organization, to the action of any change in the conditions of life. Nothing is more easy than to tame an animal, and few things more difficult than to get it to breed freely under confinement, even in the many cases when the male and female unite. How many animals there are which will not breed, though living long under not very close confinement in their native country! This is generally attributed to vitiated instincts; but how many cultivated plants display the utmost vigour, and yet rarely or never seed! In some few such cases it has been found out that very trifling changes, such as a little more or less water at some particular period of growth, will determine whether or not the plant sets a seed. I cannot here enter on the copious details which I have collected on this curious subject; but to show how singular the laws are which determine the reproduction of animals under confinement, I may just mention that carnivorous animals, even from the tropics, breed in this country pretty freely under confinement, with the exception of the plantigrades or bear family; whereas, carnivorous birds, with the rarest exceptions, hardly ever lay fertile eggs. Many exotic plants have pollen utterly worthless, in the same exact condition as in the most sterile hybrids. When, on the one hand, we see domesticated animals and plants, though often weak and sickly, yet breeding quite freely under confinement; and when, on the other hand, we see individuals, though taken young from a state of nature, perfectly tamed, long-lived, and healthy (of which I could give numerous instances), yet having their reproductive system so seriously affected by unperceived causes as to fail in acting, we need not be surprised at this system, when it does act under confinement, acting not quite regularly, and producing offspring not perfectly like their parents or variable.</p><p>Sterility has been said to be the bane of horticulture; but on this view we owe variability to the same cause which produces sterility; and variability is the source of all the choicest productions of the garden. I may add, that as some organisms will breed most freely under the most unnatural conditions (for instance, the rabbit and ferret kept in hutches), showing that their reproductive system has not been thus affected; so will some animals and plants withstand domestication or cultivation, and vary very slightly perhaps hardly more than in a state of nature.</p><p>A long list could easily be given of ''sporting plants;'' by this term gardeners mean a single bud or offset, which suddenly assumes a new and sometimes very different character from that of the rest of the plant. Such buds can be propagated by grafting, &c., and sometimes by seed. These ''sports'' are extremely rare under nature, but far from rare under cultivation; and in this case we see that the treatment of the parent has affected a bud or offset, and not the ovules or pollen. But it is the opinion of most physiologists that there is no essential difference between a bud and an ovule in their earliest stages of formation; so that, in fact,''sports'' support my view, that variability may be largely attributed to the ovules or pollen, or to both, having been affected by the treatment of the parent prior to the act of conception. These cases anyhow show that variation is not necessarily connected, as some authors have supposed, with the act of generation...</p>",
                Title = "Variation Under Domestication",
                ID = 3,
                UserId = 1,
                UpdateDate = DateTime.Now,
                isActive = true
            });
            context.UserPosts.Add(new UserPost
            {
                CreateDate = DateTime.Now,
                Post = "<p>BEFORE applying the principles arrived at in the last chapter to organic beings in a state of nature, we must briefly discuss whether these latter are subject to any variation. To treat this subject at all properly, a long catalogue of dry facts should be given; but these I shall reserve for my future work. Nor shall I here discuss the various definitions which have been given of the term species. No one definition has as yet satisfied all naturalists; yet every naturalist knows vaguely what he means when he speaks of a species. Generally the term includes the unknown element of a distinct act of creation. The term ''variety'' is almost equally difficult to define; but here community of descent is almost universally implied, though it can rarely be proved. We have also what are called monstrosities; but they graduate into varieties. By a monstrosity I presume is meant some considerable deviation of structure in one part, either injurious to or not useful to the species, and not generally propagated. Some authors use the term ''variation'' in a technical sense, as implying a modification directly due to the physical conditions of life; and ''variations'' in this sense are supposed not to be inherited: but who can say that the dwarfed condition of shells in the brackish waters of the Baltic, or dwarfed plants on Alpine summits, or the thicker fur of an animal from far northwards, would not in some cases be inherited for at least some few generations? and in this case I presume that the form would be called a variety.</p><p>Again, we have many slight differences which may be called individual differences, such as are known frequently to appear in the offspring from the same parents, or which may be presumed to have thus arisen, from being frequently observed in the individuals of the same species inhabiting the same confined locality. No one supposes that all the individuals of the same species are cast in the very same mould. These individual differences are highly important for us, as they afford materials for natural selection to accumulate, in the same manner as man can accumulate in any given direction individual differences in his domesticated productions. These individual differences generally affect what naturalists consider unimportant parts; but I could show by a long catalogue of facts, that parts which must be called important, whether viewed under a physiological or classificatory point of view, sometimes vary in the individuals of the same species. I am convinced that the most experienced naturalist would be surprised at the number of the cases of variability, even in important parts of structure, which he could collect on good authority, as I have collected, during a course of years. It should be remembered that systematists are far from pleased at finding variability in important characters, and that there are not many men who will laboriously examine internal and important organs, and compare them in many specimens of the same species. I should never have expected that the branching of the main nerves close to the great central ganglion of an insect would have been variable in the same species; I should have expected that changes of this nature could have been effected only by slow degrees: yet quite recently Mr Lubbock has shown a degree of variability in these main nerves in Coccus, which may almost be compared to the irregular branching of the stem of a tree. This philosophical naturalist, I may add, has also quite recently shown that the muscles in the larvae of certain insects are very far from uniform. Authors sometimes argue in a circle when they state that important organs never vary; for these same authors practically rank that character as important (as some few naturalists have honestly confessed) which does not vary; and, under this point of view, no instance of any important part varying will ever be found: but under any other point of view many instances assuredly can be given.</p>",
                Title = "Variation Under Nature",
                ID = 4,
                UserId = 1,
                UpdateDate = DateTime.Now,
                isActive = true
            });

            base.Seed(context);
        }
    }
}