using Microsoft.AspNetCore.Components;
using ReplyApp.Models;
using System.Threading.Tasks;

namespace ReplyApp.Pages.Replys
{
    public partial class Details
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IReplyRepository ReplyRepositoryAsyncReference { get; set; }

        protected Reply model = new Reply();

        protected string content = "";

        protected override async Task OnInitializedAsync()
        {
            model = await ReplyRepositoryAsyncReference.GetByIdAsync(Id);
            content = Dul.HtmlUtility.EncodeWithTabAndSpace(model.Content);
        }
    }
}
