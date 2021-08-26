using FluentFTP;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FtpFileCommunication.Service
{
    public class UploadFilesService
    {
        public static async Task UploadFilesAsync(string[] path,
            string host, int port, string username, string pass)
        {
            var token = new CancellationToken();
            using (var ftp = new FtpClient(host, port, username, pass))
            {
                await ftp.ConnectAsync(token);
                await CreateDirectoryNotExist(host, port, username, pass, "/OUT");

                await ftp.UploadFilesAsync(
                    path,
                    "/OUT", FtpRemoteExists.Skip);
            }
        }

        public static async Task UploadFilesPdf(string[] path,
            string host, int port, string username, string pass)
        {
            // krijon dhe lidhet me klientin FTP
            var ftp = new FtpClient(host, port,
                new NetworkCredential(username, pass))
            {
                ValidateAnyCertificate = true,
            };

            ftp.Connect();

            if (!ftp.DirectoryExists("/PDF"))
            {
                ftp.CreateDirectory("/PDF", true);
            }

            ftp.UploadFiles(path, "/PDF", FtpRemoteExists.Skip);
            ftp.Disconnect();
        }

        private static void OnValidateCertificate(FtpClient control,
            FtpSslValidationEventArgs e)
        {
            // add logic to test if certificate is valid here
            e.Accept = true;
        }

        public static async Task CreateDirectoryAsync(string host,
            int port, string username, string pass, string directory)
        {
            var token = new CancellationToken();
            using (var conn = new FtpClient(host, port, username, pass))
            {
                conn.CreateDirectory(directory, true);
            }
        }

        public static async Task CreateDirectoryNotExist(string host,
            int port, string username, string pass, string directory)
        {
            using (var conn = new FtpClient(host, port, username, pass))
            {
                if (!(conn.DirectoryExists(directory)))
                {
                    await CreateDirectoryAsync(host, port, username, pass, directory);
                }
            }
        }

        public static async Task DeleteDirectoryAsync(string host, int port,
            string username, string pass, string directory)
        {
            var token = new CancellationToken();
            using (var conn = new FtpClient(host, port, username, pass))
            {
                await conn.ConnectAsync(token);

                // Remove the directory and all files and subdirectories inside it
                await conn.DeleteDirectoryAsync(directory, token);
            }
        }

        public static async Task DeleteFileAsync(string host, int port,
            string username, string pass, string path)
        {
            var token = new CancellationToken();
            using (var conn = new FtpClient(host, port, username, pass))
            {
                //await conn.ConnectAsync(token);
                conn.Connect();

                // Remove the file
                if (conn.FileExists(path))
                {
                    conn.DeleteFile(path);
                }
            }
        }

        public static async Task UploadFileAsync(string path, string fileName,
            string host, int port, string username, string pass)
        {
            // krijon dhe lidhet me klientin FTP
            var ftp = new FtpClient(host, port,
                new NetworkCredential(username, pass))
            {
                ValidateAnyCertificate = true
            };
            ftp.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);

            await ftp.ConnectAsync();

            await CreateDirectoryNotExist(host, port, username, pass, "/OUT");

            ftp.UploadFile(
                path,
                "/OUT/" + fileName, FtpRemoteExists.Skip);
            ftp.Disconnect();
        }

        public static async Task DownloadDirectoryAsync(string host, 
            int port, string username, string pass, string pathOut)
        {
            var token = new CancellationToken();
            using (var ftp = new FtpClient(host, port, username, pass))
            {
                await ftp.ConnectAsync(token);
                await CreateDirectoryNotExist(host, port, username, pass, "/IN");

                // download a folder and all its files
                await ftp.DownloadDirectoryAsync(pathOut, @"/IN", FtpFolderSyncMode.Update);
            }
        }
    }
}