# Ideas

Load everything at once save things in smaller chunks?

```rust

pub struct EditorModel {
    pub entity: Entity { // This can be any struct, with struct children
        pub entity_id: i32,
        pub name: String,
        pub dob: String,
        // etc...
    }
    pub load_request: EntityLoadRequest { 
        pub entity_id: i32,       // <--- I'm not sure if this is useful since we use query params normally
        // Anything else too
    },
    pub load_result: EntityLoadResult {
        pub entity: Entity,
        pub status: Loadtatus { // Identital to LoadStatus
            pub success: bool,
            pub errors: Vec<String>,
            pub has_error: bool, // I guess this is just errors.len() > 0  
            pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
        },
    },
    pub save_user_request:  EntitySaveRequest {
        pub entity: Entity,
        pub created_by: Option<String>,
        // Anything else we need to add
    },
    pub save_user_result: EntitySaveResult {
        pub entity: Entity,  // A copy of the entity saved, with things added like id numbers and creation dates
        pub status: SaveStatus { // Identital to LoadStatus
            pub success: bool,
            pub errors: Vec<String>,
            pub has_error: bool, // I guess this is just errors.len() > 0  
            pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
        },
    },

    pub save_user_group_request: SaveUserGroupRequest { 
        pub user_group_id: String
    }, 
    pub save_user_group_result: SaveUserGroupResult { 
        pub entity: Entity, // Maybe a copy of the whole entity?
        pub user_group_id, // The thing changed
        pub user_group_description, // An expanded version of the thing changed
        pub status: SaveStatus { // Identital to LoadStatus
            pub success: bool,
            pub errors: Vec<String>,
            pub has_error: bool, // I guess this is just errors.len() > 0  
            pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
        }
    },
}

pub struct Entity { // This can be any struct, with struct children
    pub entity_id: i32,
    pub name: String,
    // etc...
}

pub struct EntitySaveRequest {
    pub entity: Entity,
    pub created_by: Option<String>,
    // Anything else we need to add
}

pub struct EntitySaveResult {
    pub entity: Entity,  // A copy of the entity saved, with things added like id numbers and creation dates
    pub status: SaveStatus,
}

pub struct SaveStatus { // Identital to LoadStatus
    pub success: bool,
    pub errors: Vec<String>,
    pub has_error: bool, // I guess this is just errors.len() > 0  
    pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
}

pub struct EntityLoadRequest {
    pub entity_id: i32,
    // Anything else?
}

pub struct EntityLoadResult {
    pub entity: Entity,
    pub status: LoadStatus,
}

pub struct LoadStatus { // Identitcal to SaveStatus
    pub success: bool,
    pub errors: Vec<String>,
    pub has_error: bool, // I guess this is just errors.len() > 0  
    pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
}

```