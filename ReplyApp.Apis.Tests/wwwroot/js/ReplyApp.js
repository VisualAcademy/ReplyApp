console.log("ReplyApp.Apis REST API Test");

//[0] Web API URL
var url = "https://localhost:44350/api/Replys";

//[1] Fetch API 사용하기
//console.log(fetch(url));

//[2] Fetch API 결괏값 사용
//fetch(url).then(response => console.log(response));
//fetch(url)
//    .then(response => response.json())
//    .then(data => console.log(data));

////[3] Fetch API로 데이터 전송
//fetch(url, {
//    method: "POST",
//    headers: {
//        "Content-Type": "application/json"
//    },
//    body: JSON.stringify({
//        name: 'FetchTest',
//        title: 'FetchTest'
//    })
//}).then(response => {
//      if (response.ok) {
//          console.log("성공");
//          return response.json();
//      }
//      else {
//          console.log("에러");
//      }
//  })
//  .then(data => console.log(data))
//  .catch(error => console.log(error));

//[4] 토큰을 전달해서 인증이 필요한 Web API 호출 테스트
fetch(url, {
    method: "GET",
    headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkEzRXZnbEZNMFlhMDFPLVQ1NFNJc3ciLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE1ODc2NTIyOTksImV4cCI6MTU4NzY1NTg5OSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiUmVwbHlBcHAuQXBpcyIsImNsaWVudF9pZCI6IlJlcGx5QXBwLk1vZGVscy5UZXN0cyIsInNjb3BlIjpbIlJlcGx5QXBwLkFwaXMiXX0.nZwJkNoA9t1x-tjWC5Kp-eeueHDKTgnHdkhKiBk16AqKtsLdDxCi7OyOtrx0_wf4J5gBmeBIgW0Sb0KL935bRniTl_C5FbebUqwUBArFa4SwJBWLjr8T-w3ScaSSTmgGbC_UNF42X7-4KLs-JnzwTSoHqPzK6NloKnWtFn0l3ezE9fgl0101fNfcKU1wYnqP46oeRq7ND14WVp90WiE51KPCIdzaqBTqkrBzI-XiGB3PwlKS-PRszgAZ0kxz_KM8ddky-MtE84WzuQOpWtCsXZMd7mlogYkrvs4D3Bth-wCqQqC8ikKn6ZTcYPzbpSbulQSn7lu6oe4UKsvljtyy5g"
    }
})
    .then(response => response.json())
    .then(data => console.log(data));
