using Microsoft.AspNetCore.Components;
using NoticeApp.Models;
using System.Threading.Tasks;

namespace ReplyApp.Pages.Notices
{
    public partial class Details
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public INoticeRepositoryAsync NoticeRepositoryAsyncReference { get; set; }

        protected Notice model = new Notice();

        protected string content = "";

        protected override async Task OnInitializedAsync()
        {
            model = await NoticeRepositoryAsyncReference.GetByIdAsync(Id);
            content = Dul.HtmlUtility.EncodeWithTabAndSpace(model.Content);
        }
    }
}
