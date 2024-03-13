# Replace permissions on all files and directory

Source link: https://stackoverflow.com/questions/3740152/how-do-i-change-permissions-for-a-folder-and-its-subfolders-files

```
The other answers are correct, in that chmod -R 755 will set these permissions to all files and subfolders in the tree. But why on earth would you want to? It might make sense for the directories, but why set the execute bit on all the files?

I suspect what you really want to do is set the directories to 755 and either leave the files alone or set them to 644. For this, you can use the find command. For example:

To change all the directories to 755 (drwxr-xr-x):

find /opt/lampp/htdocs -type d -exec chmod 755 {} \;
To change all the files to 644 (-rw-r--r--):

find /opt/lampp/htdocs -type f -exec chmod 644 {} \;
Some splainin': (thanks @tobbez)

chmod 755 {} specifies the command that will be executed by find for each directory
chmod 644 {} specifies the command that will be executed by find for each file
{} is replaced by the path
; the semicolon tells find that this is the end of the command it's supposed to execute
\; the semicolon is escaped, otherwise it would be interpreted by the shell instead of find
```