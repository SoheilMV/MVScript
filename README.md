# MVScript
is a library designed for creating, managing, and interacting with structured information in readable and maintainable script formats. This library utilizes a hierarchical structure to represent information, consisting of roads, blocks, and key-value pairs.

## Features
1. Text Reading and Parsing: Capability to read a text and parse it to create roads, blocks, and relevant information.
2. Accessing Information Using Indexers: Access roads and blocks using indexers.
3. Structured Data Storage: Information is stored in a structured tree-like format using the **Road**, **Block**, and **Value** classes.
4. String Conversion: Ability to convert information to a string while maintaining the original structure.

## Key Components
1. **Road:** Represents a path in the structured information. Each road contains blocks and additional information.
2. **Block:** Exists within a road and holds key-value pairs. Each block may contain nested blocks and simple information.
3. **Key-Value Pair:** Each block contains pairs of information represented in key-value format.

## Example Script
```
BEGIN ROAD road1
        BEGIN BLOCK block1
                name: Soheil MV
                date: 2023
                isDev: true
        END BLOCK
        BEGIN BLOCK block2
                myList: [Soheil MV, 2023, true]
        END BLOCK
        BEGIN BLOCK block3
                myDictionary: {name:Soheil MV; date:2023; isDev:true}
        END BLOCK
END ROAD
BEGIN ROAD road2
        BEGIN BLOCK block1
                myList: [{name:Soheil MV; date:2023; IsDev:true}, 200, true]
        END BLOCK
END ROAD
BEGIN ROAD road3
        BEGIN BLOCK block1
                myDictionary: {myList:[Soheil MV, 2023, true]; StatusCode:200; IsOk:true}
        END BLOCK
END ROAD
```