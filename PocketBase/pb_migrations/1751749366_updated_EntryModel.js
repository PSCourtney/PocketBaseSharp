/// <reference path="../pb_data/types.d.ts" />
migrate((app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // update collection data
  unmarshal({
    "name": "Entry"
  }, collection)

  return app.save(collection)
}, (app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // update collection data
  unmarshal({
    "name": "EntryModel"
  }, collection)

  return app.save(collection)
})
