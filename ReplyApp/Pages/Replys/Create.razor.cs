using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using ReplyApp.Models;
using ReplyApp.Managers;
using System.Threading.Tasks;

namespace ReplyApp.Pages.Replys
{
    public partial class Create
    {
        [Parameter]
        public int Id { get; set; } = 0; 

        [Inject]
        public IReplyRepository RepositoryReference { get; set; }

        [Inject]
        public NavigationManager NavigationManagerReference { get; set; }

        protected Reply model = new Reply();

        public string ParentId { get; set; }

        protected int[] parentIds = { 1, 2, 3 };

        // 부모 글의 Id를 임시 보관
        public int ParentRef { get; set; } = 0;
        public int ParentStep { get; set; } = 0;
        public int ParentRefOrder { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            if (Id != 0)
            {
                // 기존 글의 데이터를 읽어오기 
                model = await RepositoryReference.GetByIdAsync(Id);
                model.Id = 0; 
                model.Name = "";
                model.Title = "Re: " + model.Title;
                model.Content = "\r\n====\r\n" + model.Content;

                ParentRef = (int)model.Ref; 
                ParentStep = (int)model.Step; 
                ParentRefOrder = (int)model.RefOrder;
            }
        }

        protected async void FormSubmit()
        {
            int.TryParse(ParentId, out int parentId);
            model.ParentId = parentId;

            #region 파일 업로드 관련 추가 코드 영역
            if (selectedFiles != null && selectedFiles.Length > 0)
            {
                // 파일 업로드
                var file = selectedFiles.FirstOrDefault();
                var fileName = "";
                int fileSize = 0;
                if (file != null)
                {
                    fileName = file.Name;
                    fileSize = Convert.ToInt32(file.Size);

                    fileName = await FileStorageManager.UploadAsync(file.Data, file.Name, "", true);

                    model.FileName = fileName;
                    model.FileSize = fileSize;
                } 
            }
            #endregion

            if (Id != 0)
            {
                // 답변 글이라면,
                await RepositoryReference.AddAsync(model, ParentRef, ParentStep, ParentRefOrder);
            }
            else
            {
                // 일반 작성 글이라면,
                await RepositoryReference.AddAsync(model);
            }

            NavigationManagerReference.NavigateTo("/Replys");
        }

        [Inject]
        public IFileStorageManager FileStorageManager { get; set; }
        private IFileListEntry[] selectedFiles;
        protected void HandleSelection(IFileListEntry[] files)
        {
            this.selectedFiles = files;
        }
    }
}
