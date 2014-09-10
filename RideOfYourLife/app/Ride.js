var rideApp = angular.module('RideApp', ['ui.bootstrap', 'ngAnimate', 'ui.router']);


rideApp.directive('markdown', function() {
    var converter = new Showdown.converter();
    var link = function(scope, element, attrs, model) {
        var render = function() {
            var htmlText = converter.makeHtml(model.$modelValue);
            element.html(htmlText);
        };
        scope.$watch(attrs['ngModel'], render);
        render();
    };
    return {
        restrict: 'E',
        require: 'ngModel',
        link: link
    };
});



//rideApp.config(["$locationProvider", function ($locationProvider) {
//    $locationProvider.html5Mode(true); // this works, but it doesn't work quite well: stackoverflow.com/questions/22102815/how-to-delete-sign-in-angular-ui-router-urls#comment33528392_22102901
//}]);


// http://jsfiddle.net/8bENp/66/

//rideApp.directive("markdown", function ($compile, $http) {
//    var converter = new Showdown.converter();
//    return {
//        restrict: 'E',
//        replace: true,
//        link: function (scope, element, attrs) {
//            if ("src" in attrs) {
//                $http.get(attrs.src).then(function (data) {
//                    element.html(converter.makeHtml(data.data));
//                });
//            } else {
//                element.html(converter.makeHtml(element.text()));
//            }
//        }
//    };
//});



rideApp.service('LanguageService', function () {

    this.languageCultureCode = "en";

    this.setLanguage = function (languageCultureCode) {
        this.languageCultureCode = languageCultureCode;
    };

    this.getLanguage = function() {
        return this.languageCultureCode;
    };

});



rideApp.config(['$httpProvider', function ($httpProvider) {
    // $httpProvider.defaults.useXDomain = true;
    
    $httpProvider.defaults.withCredentials = true;

    // delete $httpProvider.defaults.headers.common['X-Requested-With'];
    //delete $httpProvider.defaults.headers.post['Content-type']; // http://stackoverflow.com/questions/15490256/why-does-a-cross-domain-angularjs-http-post-request-fail-when-a-xmlhttprequest
}]);


var reservationFormController = ['$scope', '$http', '$state', 'LanguageService',
    function ($scope, $http, $state, LanguageService) {
        
        

    $scope.alreadySubmitted = false;

    $scope.message = null;

    $scope.resource = null;

    $scope.ageBrackets = null;

    $scope.nationalities = null;

    $scope.reservation = {
        titleId: 1, // defaults to Mistersw
        guestName: "",
        emailAddress: "",
        contactNumber: "",
        nationalityId: null,
        dateOfRide: null,

        reservationAdditionals: [] // { GuestName : string, AgeBracketId : int, NationalityId : int }
    };


    $scope.removeOtherReservation = function(index) {
        $scope.reservation.reservationAdditionals.splice(index, 1);
    };

    $scope.finishedLoadingResources = false;


    $scope.validate = function() {

        


        $scope.alreadySubmitted = true;

        // http://stackoverflow.com/questions/18324773/force-ng-dirty-in-angularjs-form-validation

        for (var field in $scope.reservationHeaderForm) {
            var control = $scope.reservationHeaderForm[field];
            // look at each form input with a name attribute set
            // checking if it is pristine and not a '$' special field
            if (field[0] != '$' && control.$pristine) {
                control.$setViewValue(
                    control.$modelValue
                );
            }
        }
        
        
        if (!$scope.reservationHeaderForm.$valid)
            return false;


        for (var i in $scope.reservation.reservationAdditionals) {
            var a = $scope.reservation.reservationAdditionals[i];

            if (a.nationalityId == null)
                return false;
        }

        return true;

    };


    $scope.addGuest = function() {

        $scope.reservation.reservationAdditionals.push({ guestName: "", ageBracketId: 1, nationalityId: null });
    };


    // http://stackoverflow.com/questions/19343316/angularjs-and-cross-domain-post
  

    $scope.fetchResource = function() {

        return $http({ method: 'GET', url: 'http://localhost:1337/reservation-resource', params: { languageCultureCode: LanguageService.getLanguage() } }).
            success(function(data, status, headers, config) {
                $scope.resource = data;
            }).
            error(function(data, status, headers, config) {
                
            });
    };

    $scope.fetchTitles = function() {

        return $http({ method: 'GET', url: 'http://localhost:1337/titles', params: { languageCultureCode: LanguageService.getLanguage() } }).
            success(function(data, status, headers, config) {
                $scope.titles = data;
            }).
            error(function(data, status, headers, config) {
                
            });

    };


    $scope.fetchNationalities = function() {

        return $http({ method: 'GET', url: 'http://localhost:1337/nationalities', params: { languageCultureCode: LanguageService.getLanguage() } }).
            success(function(data, status, headers, config) {
                $scope.nationalities = data;
            }).
            error(function(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
    };


    $scope.fetchAgeBrackets = function() {

        return $http({ method: 'GET', url: 'http://localhost:1337/age-brackets', params: { languageCultureCode: LanguageService.getLanguage() } }).
            success(function(data, status, headers, config) {
                $scope.ageBrackets = data;
            }).
            error(function(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
    };

    $scope.$on('handleGeneralBroadcast', function () {
        $scope.switchResourcesLanguage();        
    });

    $scope.switchResourcesLanguage = function() {

        $scope.fetchResource().
            then($scope.fetchTitles).
            then($scope.fetchNationalities).
            then($scope.fetchAgeBrackets).
            then(function() {
                $scope.finishedLoadingResources = true;
            });

    };

        

    $scope.reserve = function() {

        var isValid = $scope.validate();

        if (!isValid) return;

        return $http({ method: 'POST', url: 'http://localhost:1337/reservation', data: $scope.reservation }).
            success(function(data, status, headers, config) {                
                $state.go('viewReservedState', { reservationId: data.reservationId });                
            }).
            error(function(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
    };


    // datepicker
    $scope.open = function($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.opened = true;
    };
        

        /// main
        
    $scope.switchResourcesLanguage();

    
}]; // reservationFormController


var viewReservedController = ['$scope', '$http', '$state', '$stateParams', 'LanguageService', function ($scope, $http, $state, $stateParams, LanguageService) {
    //alert($stateParams.reservationId);

    // alert($stateParams.reservationId)

    $scope.reservation = {};

    $scope.finishedLoading = false;
    
    

    $scope.fetchResource = function () {

        return $http({ method: 'GET', url: 'http://localhost:1337/reservation-resource', params: { languageCultureCode: LanguageService.getLanguage() } }).
            success(function (data, status, headers, config) {
                $scope.resource = data;
            }).
            error(function (data, status, headers, config) {

            });
    };

    
    $scope.fetchViewVersion = function() {
        return $http({ method: 'GET', url: 'http://localhost:1337/reservation/' + $stateParams.reservationId + '/' + LanguageService.getLanguage() }).
            success(function (data, status, headers, config) {
                $scope.reservation = data;
            }).
            error(function (data, status, headers, config) {

            });
    };

    $scope.fetchAll = function() {
        $scope.fetchResource().then($scope.fetchViewVersion).then(function() {
            $scope.finishedLoading = true;
        });

    };
   
    $scope.$on('handleGeneralBroadcast', function () {
        $scope.fetchAll();
    });


    $scope.goToReservationForm = function() {
        $state.go('reservationState');
    };


    $scope.fetchAll();
}];


rideApp.config(['$stateProvider','$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/reservation-form");

    $stateProvider
        .state('mainState', {
            url: "/main",
            templateUrl: "Partials/Main.html"
        })
        .state('xyzState', {
            url: "/xyz",
            templateUrl: "Partials/Xyz.html"
        })
        .state('reservationState', {
            url: "/reservation-form",
            views: {
                'mainPortion': {
                    templateUrl: "Partials/ReservationForm.html",
                    controller: reservationFormController
                },
                'navigationPortion' : {
                    templateUrl: 'Partials/NavigationViewReserved.html',
                    controller: function($scope) {
                        $scope.message = "Cowabunga!";
                    }
                }
            }            
        })
        .state('viewReservedState', {
            url: "/view-reserved/:reservationId", 
            views: {
                'mainPortion': {
                    templateUrl: "Partials/ViewReserved.html",
                    controller: viewReservedController
                }
                //, 'navigationPortion' : {
                //    templateUrl: 'Partials/NavigationViewReserved.html',
                //    controller: function($scope) {
                        
                //    }
                //}
            }
            
        });
}]);


rideApp.controller('MainController',['$scope', '$http', 'LanguageService', 'BroadcastService', function($scope, $http, LanguageService, BroadcastService) {
    $scope.testMessage = "Hello";
    $scope.currentLanguage = "en";

    $scope.availableLanguages = [
        { languageCultureCode: "en", language: "English" },
        { languageCultureCode: "zh", language: "Chinese" }
    ];


    $scope.switchLanguage = function (isFromDropdown) {
        
        if (isFromDropdown == undefined) {
            $scope.fetchLanguage().then(function () {
                BroadcastService.broadcast();

            });
        } else {
            $scope.changeLanguage($scope.currentLanguage).then(function() {
                BroadcastService.broadcast();
            });
            
        }
        
    };
    

    $scope.fetchLanguage = function () {
        return $http({ method: 'GET', url: 'http://localhost:1337/language' }).
            success(function (data, status, headers, config) {
                // $scope.currentLanguage = data.languageCultureCode;
                // LanguageService.setLanguage($scope.currentLanguage);
                //$scope.currentLanguage = data.languageCultureCode;
                $scope.currentLanguage = data.languageCultureCode;
                LanguageService.setLanguage(data.languageCultureCode);
                
            }).
            error(function (data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
    };


    $scope.changeLanguage = function (languageCultureCode) {
        return $http({
                method: 'POST',
                url: 'http://localhost:1337/language',
                data: { languageCultureCode: languageCultureCode }            
            }).
            success(function (data, status, headers, config) {
            }).
            error(function (data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            }).then($scope.fetchLanguage); // To ensure that it was really set in the service
    };



    $scope.switchLanguage(); // defaults to en, if no language set yet


}]);



// http://stackoverflow.com/questions/23967785/angular-ui-ui-router-passing-data-and-calling-functions-on-child-ui-view
rideApp.factory('BroadcastService', ['$rootScope', function ($rootScope) {
    var broadcastService = {};

    broadcastService.broadcast = function () {
        $rootScope.$broadcast('handleGeneralBroadcast');
    };

    return broadcastService;
}]);