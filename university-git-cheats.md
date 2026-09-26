Шпаргалка:

```
student@ilab:~/CsLabs/Kirsanov$ git clone https://github.com/StriderAJR/git-demo.git
student@ilab:~/CsLabs/Kirsanov$ cd git-demo/
student@ilab:~/CsLabs/Kirsanov/git-demo$ git config user.name "StriderAJR"
student@ilab:~/CsLabs/Kirsanov/git-demo$ git config user.email "alexander.aj.ranger@gmail.com"
student@ilab:~/CsLabs/Kirsanov/git-demo$ unset GIT_ASK_PASS
student@ilab:~/CsLabs/Kirsanov/git-demo$ unset SSH_ASKPASS
student@ilab:~/CsLabs/Kirsanov/git-demo$ unset SSH_ASKPASS_REQUIRE
student@ilab:~/CsLabs/Kirsanov/git-demo$ export GIT_TERMINAL_PROMPT=1
student@ilab:~/CsLabs/Kirsanov/git-demo$ git -c core.askPass= -c credential.helper= push
Username for 'https://github.com': StriderAJR
Password for 'https://StriderAJR@github.com': 
Everything up-to-date
```
