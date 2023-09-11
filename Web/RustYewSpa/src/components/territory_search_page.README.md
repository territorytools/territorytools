# Data Flow

## Sequence of Events for **editor**:
- create
    - send_future(RefreshFromSearchText())
    - return empty Self struct
- view
    - search_text = search_text from query param
    - html!

- render: not implemented

- update: Messages
  - RefreshFromSearchText()
    - search_text = search_text query param
    - send_future(Load(fetch_data(search_text)))
  - Load(FetchDataResult)

- onchange: search text input Callback
    - get value from HTML input box
    - navigator.push_with_query(with value)
    - (The above push triggers the _listener)

- _listener: location change listener Callback
    - send_message(RefreshFromSearchText())

