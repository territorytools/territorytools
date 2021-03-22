$html = "<html><body><table>"
#$html += "<th><td>Number</td><td>User</td><td>UserId</td></th>"
$html += "<tr>" +
  "<th>Number</th>" +
  "<th>User</th>" +
  "<th>UserId</th>" +
  "<th>Description</th>" +
  "</tr>"
$html += $territories |
  Where SignedOutTo -ne $null |
  Sort SignedOut |
  Select Number, Description, SignedOutTo |
  % {
        If($_.SignedOutTo -eq $null) {
          $name = ""
          $id = ""
        } Else {
          $name = $_.SignedOutTo
          $id = "$($userIds[$_.SignedOutTo])"
        };
    "<tr>" +
    "  <td>$($_.Number)</td>" +
    "  <td>$name</td>" +
    "  <td>$id</td>" +
    "  <td>$($_.Description)</td>" +
    "</tr>"
   }
$html += "</table></body></html>"
$html > test.html