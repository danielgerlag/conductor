# Installation

Conductor is available as a Docker image - `danielgerlag/conductor`

Conductor uses MongoDB as it's datastore, you will also need an instance of MongoDB in order to run Conductor.

Use this command to start a container (with the API available on port 5001) that points to `mongodb://my-mongo-server:27017/` as it's datastore.

```
$ docker run -p 127.0.0.1:5001:80/tcp --env dbhost=mongodb://my-mongo-server:27017/ danielgerlag/conductor
```

If you wish to run a fleet of Conductor nodes, then you also need to have a Redis instance, which they will use as a backplane.  This is not required if you are only running one instance.
Simply have all your conductor instances point to the same MongoDB and Redis instance, and they will operate as a load balanced fleet.

## Environment Variables to configure

You can configure the database and Redis backplane by setting environment variables.

```
dbhost: <<insert connection string to your MongoDB server>>
redis: <<insert connection string to your Redis server>> (optional)
```

If you would like to setup a conductor container (API on port 5001) and a MongoDB container at the same time and have them linked, use this docker compose file:

```Dockerfile
version: '3'
services:
  conductor:
    image: danielgerlag/conductor
    ports:
    - "5001:80"
    links:
    - mongo
    environment:
      dbhost: mongodb://mongo:27017/
  mongo:
    image: mongo
```
