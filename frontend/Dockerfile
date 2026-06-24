FROM node:26.3.1-alpine3.24 as build

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

RUN mkdir -p /code
WORKDIR /code
ADD . /code
RUN npm install -g -s --no-progress corepack
RUN COREPACK_ENABLE_DOWNLOAD_PROMPT=0 /bin/sh -c "yarn -v"
RUN yarn install
RUN yarn run build

FROM httpd:2.4.68-alpine3.24
COPY --from=build /code/dist /usr/local/apache2/htdocs/