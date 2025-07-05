/// <reference path="../pb_data/types.d.ts" />
migrate((app) => {
  const collection = app.findCollectionByNameOrId("pbc_1036457598")

  // update collection data
  unmarshal({
    "name": "todos"
  }, collection)

  return app.save(collection)
}, (app) => {
  const collection = app.findCollectionByNameOrId("pbc_1036457598")

  // update collection data
  unmarshal({
    "name": "ToDo"
  }, collection)

  return app.save(collection)
})
