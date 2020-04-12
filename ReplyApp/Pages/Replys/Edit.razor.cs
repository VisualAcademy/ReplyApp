using Microsoft.AspNetCore.Components;
using ReplyApp.Models;
using System.Threading.Tasks;
using ReplyApp.Managers;
using BlazorInputFile;
using System.Linq;
using System;

namespace ReplyApp.Pages.Replys
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public int Id { get; set; }
        #endregion

        #region Injectors
        [Inject]
        public IReplyRepository RepositoryReference { get; set; }

        [Inject]
        public NavigationManager NavigationManagerInjector { get; set; } 

        [Inject]
        public IFileStorageManager FileStorageManagerInjector { get; set; }
        #endregion

        protected Reply model = new Reply();

        public string ParentId { get; set; }

        /// <summary>
        /// 부모(카테고리) 리스트가 저장될 임시 변수
        /// </summary>
        protected int[] parentIds = { 1, 2, 3 };

        protected string content = "";

        /// <summary>
        /// 페이지 초기화 이벤트 처리기
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            model = await RepositoryReference.GetByIdAsync(Id);
            content = Dul.HtmlUtility.EncodeWithTabAndSpace(model.Content);
            ParentId = model.ParentId.ToString(); 
        }

        /// <summary>
        /// 수정 버튼 이벤트 처리기
        /// </summary>
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

                    // 첨부 파일 삭제 
                    await FileStorageManagerInjector.DeleteAsync(model.FileName, "");

                    // 다시 업로드
                    fileName = await FileStorageManagerInjector.UploadAsync(file.Data, file.Name, "", true);

                    model.FileName = fileName;
                    model.FileSize = fileSize;
                } 
            }
            #endregion

            await RepositoryReference.EditAsync(model);
            NavigationManagerInjector.NavigateTo("/Replys");
        }

        private IFileListEntry[] selectedFiles;
        protected void HandleSelection(IFileListEntry[] files)
        {
            this.selectedFiles = files;
        }
    }
}
