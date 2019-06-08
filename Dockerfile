FROM mono

COPY . /windbot-source
WORKDIR /windbot-source
RUN xbuild /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:OutDir=/windbot/

WORKDIR /windbot
RUN curl --retry 5 --connect-timeout 30 --location --remote-header-name --remote-name https://github.com/moecube/ygopro-database/raw/master/locales/zh-CN/cards.cdb

EXPOSE 2399
CMD [ "mono", "/windbot/WindBot.exe", "ServerMode=true", "ServerPort=2399" ]
