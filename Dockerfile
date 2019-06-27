FROM mono as builder

RUN apt update && env DEBIAN_FRONTEND=noninteractive apt install -y wget git

COPY . /windbot-source
WORKDIR /windbot-source
RUN xbuild /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:OutDir=/windbot/
RUN git clone --depth=1 https://github.com/purerosefallen/ygopro-database /ygopro-database && \
	cp -rf /ygopro-database/locales/zh-CN/cards.cdb  /windbot/

FROM mono
COPY --from=builder /windbot /windbot

WORKDIR /windbot

EXPOSE 2399
CMD [ "mono", "/windbot/WindBot.exe", "ServerMode=true", "ServerPort=2399" ]
