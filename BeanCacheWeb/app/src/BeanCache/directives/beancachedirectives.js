(function() {
    'use strict';

    var beanCache = angular
        .module('beanCache');

    beanCache.directive('dashboard', function () {
        return {
            restrict: 'AE',
            templateUrl: 'src/BeanCache/view/DashBoard.html'
        };
    });

    beanCache.directive('cachemanagement', function () {
        return {
            restrict: 'AE',
            templateUrl: 'src/BeanCache/view/CacheManagement.html'
        };
    });

    beanCache.directive('settings', function () {
        return {
            restrict: 'AE',
            templateUrl: 'src/BeanCache/view/Settings.html'
        };
    });

})();