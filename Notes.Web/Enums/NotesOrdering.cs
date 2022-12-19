using System.ComponentModel.DataAnnotations;

namespace Notes.Web.Enums
{
    public enum NotesOrdering
    {
        None,
        [Display(Name = "By title alphabetically")]
        ByTitleAlphabetically,
        [Display(Name = "By title in reverse alphabetical order")]
        ByTitleReverseAlphabetically,
        [Display(Name = "By created date")]
        ByCreatedDate,
        [Display(Name = "By created date in reverse order")]
        ByCreatedDateReverse,
    }
}
