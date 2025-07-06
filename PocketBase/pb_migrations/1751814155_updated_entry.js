/// <reference path="../pb_data/types.d.ts" />
migrate((app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // update field
  collection.fields.addAt(1, new Field({
    "autogeneratePattern": "",
    "hidden": false,
    "id": "text4262580536",
    "max": 0,
    "min": 0,
    "name": "name",
    "pattern": "",
    "presentable": false,
    "primaryKey": false,
    "required": false,
    "system": false,
    "type": "text"
  }))

  // update field
  collection.fields.addAt(2, new Field({
    "hidden": false,
    "id": "bool3065835665",
    "name": "is_done",
    "presentable": false,
    "required": false,
    "system": false,
    "type": "bool"
  }))

  // update field
  collection.fields.addAt(3, new Field({
    "cascadeDelete": true,
    "collectionId": "pbc_1036457598",
    "hidden": false,
    "id": "relation2955387149",
    "maxSelect": 1,
    "minSelect": 0,
    "name": "todo_id",
    "presentable": false,
    "required": false,
    "system": false,
    "type": "relation"
  }))

  return app.save(collection)
}, (app) => {
  const collection = app.findCollectionByNameOrId("pbc_1589536405")

  // update field
  collection.fields.addAt(1, new Field({
    "autogeneratePattern": "",
    "hidden": false,
    "id": "text4262580536",
    "max": 0,
    "min": 0,
    "name": "Name",
    "pattern": "",
    "presentable": false,
    "primaryKey": false,
    "required": false,
    "system": false,
    "type": "text"
  }))

  // update field
  collection.fields.addAt(2, new Field({
    "hidden": false,
    "id": "bool3065835665",
    "name": "IsDone",
    "presentable": false,
    "required": false,
    "system": false,
    "type": "bool"
  }))

  // update field
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
})
