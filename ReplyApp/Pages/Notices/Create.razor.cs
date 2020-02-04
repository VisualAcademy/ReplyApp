using Microsoft.AspNetCore.Components;
using NoticeApp.Models;

namespace ReplyApp.Pages.Notices
{
    public partial class Create
    {
        [Inject]
        public INoticeRepositoryAsync NoticeRepositoryAsyncReference { get; set; }

        [Inject]
        public NavigationManager NavigationManagerReference { get; set; }

        protected Notice model = new Notice();

        public string ParentId { get; set; }

        protected int[] parentIds = { 1, 2, 3 };

        protected async void FormSubmit()
        {
            int.TryParse(ParentId, out int parentId);
            model.ParentId = parentId; 
            await NoticeRepositoryAsyncReference.AddAsync(model);
            NavigationManagerReference.NavigateTo("/Notices");
        }
    }
}
