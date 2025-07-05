/// <reference path="../pb_data/types.d.ts" />
migrate((app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // add field
  collection.fields.addAt(3, new Field({
    "cascadeDelete": true,
    "collectionId": "pbc_1036457598",
    "hidden": false,
    "id": "relation2955387149",
    "maxSelect": 1,
    "minSelect": 0,
    "name": "Todo_Id",
    "presentable": false,
    "required": false,
    "system": false,
    "type": "relation"
  }))

  return app.save(collection)
}, (app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // remove field
  collection.fields.removeById("relation2955387149")

  return app.save(collection)
})
