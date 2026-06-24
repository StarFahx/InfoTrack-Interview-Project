# README
This repo contains Fox's submission for InfoTrack's development project. The backend (found in `/backend`) is a dotnet 10 minimal api, and the frontend is a single-page webapp made using preact and vite.

A sample of this project will be deployed [here](http://solicitors.starfahx.com) for the next few weeks, should the steps [below](#running-the-project) not work on your machine.

## Using the App
The frontend project provides a quick way to search through the cached data. The list of firms can be filtered by name (case-insensitive, matches any part of the name) to quickly find a desired firm. Additional fields allow the user to filter results by:
- City: this field is a multi-select, and will return firms in any of the cities selected; if none are selected, it returns firms from all cities.
- Rating: this field allows the user to select a minimum rating, and a rating provider to filter by. Only firms who have a rating provided by the selected provider will be returned, unless the minimum rating field is de-selected.

These fields are disabled if the option to search by name only is selected.

Finally, the user can change the ordering of results and the number displayed per page.

Clicking on a result brings up a modal display of all the relevant ratings and contact information available for the firm.

## Running the Project
The project is designed to be run using `docker compose`. Clone the project, and add a `compose.yaml` file in the root directory. A sample compose file might look like this:

```yaml
services:
  frontend:
    build:
      context: ./frontend
      args:
        VITE_API_URL: http://localhost:5123
    ports:
      - "5124:80"

  backend:
    build:
      context: ./backend
    ports:
      - "5123:8080"
    volumes:
      - "/path/to/data:/data"

```

The backend container requires a volume mapped to `/data`, which contains an `apiSettings.json` configuration file, necessary for configuring the api. A sample config file might look like this:

```json
{
  "Database": {
    "Type": "sqlite",
    "DbPath": "/data/solicitors.db"
  },
  "Imports": {
    "ImportMinutes": 5,
    "RemoveMinutes": 120,
    "StaleMinutes": 60
  }
}
```

The `Database` node must contain a `Type` field, which must have the value `"sqlite"` or `"inmemory"`, to configure a database of the appropriate kind.

For a Sqlite database, the field `"DbPath"`specifies the path to the .db file. It is recommended to choose a file in `/data`, as this is a volume already mounted and can be examined outside the container.

For an In Memory database, the field `"DbName"` specifies the name to give to the database.

The `Imports` node must contain the following fields:
-  `"ImportMinutes"`: an integer number of minutes on which to run the cache import job. The target website, solicitors.com, does not provide consistent search results, so frequent searches will ensure the most complete cache possible.
- `"RemoveMinutes"`: an integer number of minutes on which to run the cache cleanup job. This job removes stale items from the cache. It is recommended that this job run much less often than the import, for the reason above.
- `"StaleMinutes"`: an integer number of minutes since modified beyond which a cached item is considered stale, and removed by the cleanup job.

With both the configuration file and the compose file in place, running `docker compose up -d` will start the containerised applications. They can be shut down with `docker compose down`. If you've pulled a newer version of the source code, `docker compose up -d --build` should be used to relaunch the applications.

## Additional Notes on Implementation
I _somewhat_ exceeded the 1-2 hours mentioned in the document, if the amount of code here + my aversion to AI doesn't make that clear, but given in the interview it was suggested that this could take a couple of weeks I figured that was a guideline and since I didn't have much better to do for this last week I decided to put a bit of effort in. Hope that's okay! It's taken roughly a workweek to get this together, excluding time spent filing bug reports with Fresh (my first choice for a frontend framework).

I've written some notes on things I've enjoyed doing + want to show off, as well as things that have suffered for the time constraints. I then looked at how much I wrote and realised it was rather a lot. Sorry! Feel free to skim it.

### HTML Parsing
For the web-scraping part of this project, I've done my best to avoid third-party libraries as requested. To achieve this, I've written an _extremely limited_ HTML parsing library, as I wasn't able to find an easy equivalent in the standard libraries for dotnet. This library uses a design I've borrowed from compiler design (I'm currently working my way through a textbook on compiler design so that was quite handy!) where first the HTML is lexed into tokens and then those tokens are parsed in a pseudo-grammar to form a nested tree of nodes.

I made use of a C# feature here that I made use of while working through that textbook: by leveraging deferred execution, I'm able to effectively perform a single-pass parsing. I'm not sure this actually has much of a performance improvement, but it seems silly to build up a list of nodes just to immediately enumerate through it again.

Due to the time constratints, this is really quite messy code and I'm not happy with the api it provides for actually scanning through the HTML tree, but it works for now. I'd love to come back and revisit this at some point, honestly, it was a really fun little project.

### Cache Store
I've used EFCore as my ORM of choice, because it's super easy to use, and this project finally gave me the opportunity to implement something I've never had an opportunity to do in the past: runtime switching of EFCore providers! I've wanted to build an app that allows the user to define (in a compose file or similar) where the data is stored for a long time, so I'm very glad I got a chance to do that here.

It's good to know that `EnsureCreatedAsync` works the way I expected for making a new sqlite db, and avoids messing around with migrations too much. Obviously, that's only possible because the schema for this project is pretty static: solving that problem for an actively-developed version of this app, and one that uses a SQL Server or Postgres provider, would probably require a fair bit more engineering.

### General Architecture
Broadly I try to architect my projects (as can be seen here) into libraries which expose abstractions publicly and keep implementations internal, registering them with a DI container. I think this approach is cleaner than having a bunch of .Abstractions projects lying around when those aren't absolutely necessary!

The original idea was that the `Solicitors.Core` library would contain service abstractions that could be referenced by libraries for the cache store and for the cache build separately, which seems to have done the trick. This approach got a little confusing at times with the models namespaces, though, so I could probably do a better job of keeping those clean.

There's also some things that ended up in Core (e.g. the comparers for ordering results) that I don't think really belonged there, which was the result of not really having a place for API-level abstractions to belong.

Running the imports on a timer within the api is probably not how I'd want to architect this project if it were going to be developed further, but I've gone with that based on the time constraints. In an ideal world there'd be a read-only REST api app + another service app that could be scheduled by the OS. I settled on Quartz.Net to provide scheduling functionality for the import only because I'd heard of it and knew it would work, though I think it's a bit overkill for what I'm trying to do here.

### API Design
A weak point of this project, which is a shame because it's something I care quite a bit about. I'd love to take the time to do up the API nicely, handle errors + validate input etc etc, but it really seemed quite unnecessary for this project, especially since the API exists only to be consumed by the UI.

### Frontend Frameworks
As mentioned, I was going to use Deno + Fresh as my frontend framework of choice: I love preact as a lightweight, fast alternative to react, and Fresh using it by default has really won me over. Fresh's approach to server-side rendering is really fun, and I'd spent a while on this project learning all about Fresh's Partials system, which I intended to use for the results display.

Unfortunately, there's a bug in Fresh that I'd occasionally run into before that means using css/scss modules is deeply inconsistent, which is my go-to pattern if I'm not using a component library. I'm not a big fan of tailwind, and because Deno and Fresh are fairly immature it's quite hard to find a non-tailwind component/styling library that works with them.

In the end I fell back on regular preact, hosted by vite (not because I know it very well. It's just what Fresh uses under the hood for some things so I thought porting my work would be easy. It wasn't.) I've used MUI as a component library because after losing all that time I just wanted to get it done quickly, and it's a framework I've used before with some success. It's a bit dated but it gets the job done!

### (Unit) Testing
I really did not have time for this. I _should_ have absolutely done this as TDD, but given I started with the problem of the HTML Parsing and was just writing that out in a scratchpad to make something that even stood a chance of working I ended up skipping that.

I've added a couple of unit tests more to demonstrate ability and comfort with them than anything else.

I've done some reasonably thorough manual testing, and pestered friends and family to try to break the site to make sure I hadn't missed anything obvious. I'm pretty confident it'll work! But this is my first time making an app designed to be consumed by `docker compose` so it might just fall apart for you. I've tested deployment both on my desktop and on one of my servers, using various different configurations and on a couple of different Linux distributions.

## Closing Remarks
Sorry if I took too long over this. It's been nice to have something serious to work on for the last week, so I wanted to enjoy it while it lasted! Thanks for considering me for the role and I hope I get to speak with you soon.