// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract AIResponseStorage {
    struct Response {
        string query;
        string response;
        uint256 timestamp;
        address requester;
    }
    
    Response[] public responses;
    mapping(address => uint256[]) public userResponses;
    
    event ResponseStored(uint256 indexed responseId, address indexed requester);
    
    function storeResponse(string memory _query, string memory _response) public returns (uint256) {
        uint256 responseId = responses.length;
        responses.push(Response({
            query: _query,
            response: _response,
            timestamp: block.timestamp,
            requester: msg.sender
        }));
        
        userResponses[msg.sender].push(responseId);
        emit ResponseStored(responseId, msg.sender);
        return responseId;
    }
    
    function getResponse(uint256 _responseId) public view returns (
        string memory query,
        string memory response,
        uint256 timestamp,
        address requester
    ) {
        require(_responseId < responses.length, "Response does not exist");
        Response memory r = responses[_responseId];
        return (r.query, r.response, r.timestamp, r.requester);
    }
    
    function getUserResponses(address _user) public view returns (uint256[] memory) {
        return userResponses[_user];
    }
}
