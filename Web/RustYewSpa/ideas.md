# Ideas

Load everything at once save things in smaller chunks?

```rust
//load: 3 struct parameters and 1 uri
model.load_request.entity_id = 333;                   // entity_id could be user_id   
model.load_result.status  = GET model.load_request;   // Status is always the same struct, used by common functions
model.load_result.session = GET model.load_request;   // session is often different, different name, UI/User/Session
model.load_result.entity  = GET model.load_request;   // entity is always different and has a different name
if model.load_result.status.success {        
    model.session = ??? GET model.load_request;
    model.entity = model.load_result.entity.clone();
}

//save: 
if model.entity.group_id != model.load_result.entity.group_Id { //... save group_id only
    // or
if model.entity != model.load_result.entity { 
    model.save_request.entity = model.entity.clone(); 
    model.save_request.creator = me; // No, the server knows the user, not the client, it's in the headers
    //etc
    model.save_result.status = POST model.save_request; 
    
    if model.save_result.status.success {
        model.load_result.entity = model.save_result.entity.clone()
        model.entity =             model.save_result.entity.clone();
    }
}

pub struct EditorModel {
    pub entity: Entity { // This can be any struct, with struct children
        pub entity_id: i32,
        pub name: String,
        pub dob: String,
        // etc...
    }
    pub original_entity: Entity { // This would be loaded on every load, a whole copy of the entity
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
        pub entity: Entity, // <-- this can be the original_entity
        pub status: LoadStatus { // Identital to LoadStatus
            pub success: bool,
            pub errors: Vec<String>,
            pub has_error: bool, // I guess this is just errors.len() > 0  
            pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
        },
        pub session: Session {
            pub user: UserContract,
            pub features: Features,
        }
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
    pub has_error() -> bool, // I guess this is just errors.len() > 0  
    pub status_code: u16,  // Not part of JSON, but returned from HTTP code?    
    pub error_message: String,
    pub success_message: String,
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
    pub user: UserContract { //???
        pub name: String,
        pub features: String,
    }
}

```