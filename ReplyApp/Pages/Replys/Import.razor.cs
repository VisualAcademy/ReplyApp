﻿using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReplyApp.Models;
using ReplyApp.Managers;

namespace ReplyApp.Pages.Replys
{
    public partial class Import
    {
        [Inject]
        public IReplyRepository ReplyRepositoryAsyncReference { get; set; }

        [Inject]
        public NavigationManager NavigationManagerReference { get; set; }

        protected Reply model = new Reply();

        public string ParentId { get; set; }

        protected int[] parentIds = { 1, 2, 3 };

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

            foreach (var m in Models)
            {
                m.FileName = model.FileName;
                m.FileSize = model.FileSize; 
                await ReplyRepositoryAsyncReference.AddAsync(m);
            }

            NavigationManagerReference.NavigateTo("/Replys");
        }

        public List<Reply> Models { get; set; } = new List<Reply>(); 
        [Inject]
        public IFileStorageManager FileStorageManager { get; set; }
        private IFileListEntry[] selectedFiles;
        protected async void HandleSelection(IFileListEntry[] files)
        {
            this.selectedFiles = files;

            // 엑셀 데이터 읽어오기 
            if (selectedFiles != null && selectedFiles.Length > 0)
            {
                var file = selectedFiles.FirstOrDefault();

                using (var stream = new MemoryStream())
                {
                    await file.Data.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            Models.Add(new Reply
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                DownCount = int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),
                            }); ;
                        }
                    }
                }
                StateHasChanged();
            }
        }
    }
}
