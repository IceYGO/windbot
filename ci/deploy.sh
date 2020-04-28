#!/usr/bin/env bash

# Original: https://github.com/edo9300/ygopro-core/blob/master/travis/deploy.sh

set -euo pipefail

cd bin
git init
git checkout --orphan $DEPLOY_BRANCH
git config user.email deploy@travis-ci.org
git config user.name "Deployment Bot (from Travis CI)"
git add -A WindBot
git commit -qm "Deploy $DEPLOY_REPO to $DEPLOY_REPO:$DEPLOY_BRANCH"
git push -qf https://$DEPLOY_TOKEN@github.com/$DEPLOY_REPO.git $DEPLOY_BRANCH:$DEPLOY_BRANCH
