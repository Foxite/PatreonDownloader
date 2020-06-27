# PatreonDownloader
A simple command line tool that downloads all media from a Patreon campaign.

It's not exactly easy to use, but it's explained below.

Go [here](https://github.com/Foxite/PatreonDownloader/releases) to download a build.

## Usage
The program needs two pieces of information:

- The link to the /api/posts page for the creator you want to download the content of. This tells the program where to look for posts.
- Your session token. This allows the program to download everything you have access to. The program can automatically extract this cookie from supported browsers, or you may enter it yourself.

**Do not take this lightly.** Your session token can be used to gain full access to your account. As such, I encourage healthy paranoia when using this program, which is why the full source code is available.

To actually get the information:

- Link to /api/posts: Go to patreon.com and visit the posts page of the creator. Using your browser's developer tools (press F12 in most browsers), open the network inspector, and refresh the page. You will see it loading several dozen pages. Find the one that starts with "posts?". The question mark is important. Depending on your browser, it may also start with "/api/posts?". The full page address + query will be quite long. This is the string we need first.
- Session token, if you opt to enter it yourself: Also on patreon.com, on any page, get the value of the cookie named "session_id". This differs per browser, but for Chromium-based browsers it can be done by clicking on the HTTPS icon next to the page address. On Firefox, you need to view the Storage inspector in your F12 menu.

Once you have the information you can start the program, and give it the information.

The first time you run it, it will create a folder in your documents folder called PatreonDownloader, and within it, a file called posts.json, which contains the full data of all posts it downloads. At this point it does not yet download media. It is limited to downloading 20 posts at a time, but every page will be added immediately to the file, ensuring safety in the case of an unexpected crash. It will wait ten seconds between each download - this is to prevent your IP getting blocked because you are exceeding the rate limit.

The next time you run it, it will ask you what you want to do with the posts.json file. If you decide to download all images, it will create folders inside Documents/PatreonDownloader for every post, and within each folder, all media that has been downloaded for that post. Each folder will be named according to the date of the post, in YYYY-MM-DD format, and will also include the title of the post. The subfolders will contain the original files that were published, named as they were by the creator.

## External media
The program will also attempt to extract links from posts, and if possible, download the files stored at those links. Currently, only Dropbox links are supported. If you want to download external media from other hosting sites, feel free to submit a PR.

## Contributing
Please stick to existing code conventions when contributing.

To add support for additional external hosting sites, look in [LinkScraping](https://github.com/Foxite/PatreonDownloader/blob/master/PatreonDownloader/LinkScraping/) to see how it works. When you have a working downloader, add it [here](https://github.com/Foxite/PatreonDownloader/blob/master/PatreonDownloader/Program.cs#L110).

To add support for additional browsers to extract cookies from, look in [CookieExtraction](https://github.com/Foxite/PatreonDownloader/tree/master/PatreonDownloader/CookieExtraction) to see how it works. When you have a working extractor, add it [here](https://github.com/Foxite/PatreonDownloader/blob/master/PatreonDownloader/Program.cs#L20).