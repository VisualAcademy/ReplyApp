using Microsoft.AspNetCore.Components;
using NoticeApp.Models;
using System;

namespace ReplyApp.Pages.Notices.Components
{
    public partial class EditorForm
    {
        /// <summary>
        /// 모달 다이얼로그를 표시할건지 여부 
        /// </summary>
        public bool IsShow { get; set; } = false; 

        private string parentId = "0";

        protected int[] parentIds = { 1, 2, 3 };

        /// <summary>
        /// 폼 보이기 
        /// </summary>
        public void Show()
        {
            IsShow = true;
        }

        /// <summary>
        /// 폼 닫기
        /// </summary>
        public void Hide()
        {
            IsShow = false;
        }

        /// <summary>
        /// 폼의 제목 영역
        /// </summary>
        [Parameter]
        public RenderFragment EditorFormTitle { get; set; }

        /// <summary>
        /// 넘어온 모델 개체 
        /// </summary>
        [Parameter]
        public Notice Model { get; set; }

        /// <summary>
        /// 부모 컴포넌트에게 생성(Create)이 완료되었다고 보고하는 목적으로 부모 컴포넌트에게 알림
        /// </summary>
        [Parameter]
        public Action CreateCallback { get; set; }

        /// <summary>
        /// 부모 컴포넌트에게 수정(Edit)이 완료되었다고 보고하는 목적으로 부모 컴포넌트에게 알림
        /// </summary>
        [Parameter]
        public EventCallback<bool> EditCallback { get; set; }

        /// <summary>
        /// 리포지토리 클래스에 대한 참조 
        /// </summary>
        [Inject]
        public INoticeRepositoryAsync NoticeRepositoryAsyncReference { get; set; }

        protected override void OnParametersSet()
        {
            parentId = Model.ParentId.ToString();
            if (parentId == "0")
            {
                parentId = "";
            }
        }

        protected async void CreateOrEditClick()
        {
            if (!int.TryParse(parentId, out int newParentId))
            {
                newParentId = 0;
            }
            Model.ParentId = newParentId;

            if (Model.Id == 0)
            {
                // Create
                await NoticeRepositoryAsyncReference.AddAsync(Model);
                CreateCallback?.Invoke(); 
            }
            else
            {
                // Edit
                await NoticeRepositoryAsyncReference.EditAsync(Model);
                await EditCallback.InvokeAsync(true);
            }
            //IsShow = false; // this.Hide()
        }
    }
}
