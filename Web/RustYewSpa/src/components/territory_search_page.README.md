# Data Flow


## Sequence of Events for **editor**:
- create
    - send_future(RefreshFromSearchText())
    - return/set empty struct
- view
    - search_text = search_text query param
    - show data


- render: Nothing

- update/Messages (called later)
  - RefreshFromSearchText()
    - search_text = search_text query param
    - send_future(Load(fetch_data(search_text)))
  - Load(FetchDataResult)

- SearchText_OnChange: Callback
    - get value
    - navigator.push_with_query
    - ?? send_message(RefreshFromSearchText())

- LocationChangeListener: Callback
    - send_message(RefreshFromSearchText())

