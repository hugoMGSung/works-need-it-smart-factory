using System;
using System.IO;

namespace DataService
{
    public static class IO
    {
        /// <summary>
        /// 동기화 잠금
        /// </summary>
        private static readonly object syncLock = new object();

        /// <summary>
        /// 프로그램의 현재 디렉토리를 가져옵니다.
        /// C:\test\
        /// </summary>
        /// <param name="isWeb">웨프로그램여부</param>
        /// <returns></returns>
        public static string DirectoryCurrent(bool isWeb)
        {
            return System.Environment.CurrentDirectory + "\\";
        }

        /// <summary>
        /// 디렉토리 생성
        /// </summary>
        /// <param name="path">디렉토리 전체 경로 입력</param>
        public static void DirectoryCreate(string path)
        {
            // 디렉토리 추출
            path = Path.GetDirectoryName(path);

            // 하위 디렉토리가 하나씩 자동으로 생성
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 디렉토리 삭제(디렉토리 파일 및 하위 디렉토리 포함)
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryDelete(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string item in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(item))
                        File.Delete(item);
                    else
                        DirectoryDelete(item);
                }
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// 디렉토리 아래의 디렉토리 및 컨텐츠 복사
        /// Directorycopy("c:\a\", "d:\b\");
        /// </summary>
        /// <param name="sourceDir">소스폴더</param>
        /// <param name="targetDir">타겟폴더</param>
        public static void DirectoryCopy(string sourceDir, string targetDir)
        {
            // 원래 디렉토리가 존재하는 경우
            if (Directory.Exists(sourceDir))
            {
                // 대상 디렉토리가 존재하지 않으면 생성
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                // 소스 폴더 데이터 가져오기
                DirectoryInfo sourceInfo = new DirectoryInfo(sourceDir);

                // 파일 복사
                FileInfo[] files = sourceInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    File.Copy(sourceDir + "\\" + file.Name, targetDir + "\\" + file.Name, true);
                }

                // 디렉토리 복사
                DirectoryInfo[] dirs = sourceInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    string currentSource = dir.FullName;
                    string currentTarget = dir.FullName.Replace(sourceDir, targetDir);
                    Directory.CreateDirectory(currentTarget);
                    // 재귀실행
                    DirectoryCopy(currentSource, currentTarget);
                }
            }
        }

        #region 파일처리 영역

        /// <summary>
        /// 파일 저장
        /// 자동으로 디렉토리 생성 - UTF8 인코딩 사용
        /// </summary>
        /// <param name="path">전체경로</param>
        /// <param name="content">파일내용</param>
        /// <param name="isAppend">추가여부</param>
        public static string FileRead(string path)
        {
            lock (syncLock)
            {
                // 디렉토리 생성
                if (File.Exists(path))
                {
                    using (StreamReader sw = new StreamReader(path, System.Text.Encoding.UTF8))
                    {
                        string str = sw.ReadToEnd();
                        sw.Close();
                        return str;
                    }
                }
                else
                {
                    throw new Exception("파일이 존재하지 않습니다");
                }

            }
        }
        /// <summary>
        /// 파일저장
        /// 자동으로 디렉토리 생성 - UTF8 인코딩 사용
        /// </summary>
        /// <param name="path">전체경로</param>
        /// <param name="content">파일내용</param>
        /// <param name="isAppend">추가여부</param>
        public static void FileSave(string path, string content, bool isAppend = false)
        {
            lock (syncLock)
            {
                // 디렉토리 생성
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (StreamWriter sw = new StreamWriter(path, isAppend, System.Text.Encoding.UTF8))
                {
                    sw.Write(content);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 파일삭제
        /// </summary>
        /// <param name="path">파일경로</param>
        public static void FileDelete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// 파일확장자 가져오기
        /// 예: png
        /// </summary>
        /// <param name="fullName">파일전체명</param>
        /// <returns></returns>
        public static string FileNameExtension(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }

        #endregion
    }
}
