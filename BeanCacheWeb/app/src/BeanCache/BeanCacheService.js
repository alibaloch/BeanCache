/**
 * Created by prveerat on 10/2/2015.
 */
(function(){
    'use strict';

    angular.module('beanCache')
        .service('beanCacheService', ['$q', '$http','appConfig', BeanCacheService]);

    function BeanCacheService($q, $http, appConfig){

        return {
            getCache : function(key) {

                return $http.get(appConfig.asfAPIUrl + "/beancache/get?key="+key)
                    .success(function(data) {
                        return data;
                    });

            },

            getAllCache : function(numberOfRecords) {

                return $http.get(appConfig.asfAPIUrl + "/cachemanagement/get?numberOfRecords="+numberOfRecords)
                    .success(function(data) {
                        return data;
                    });

            },

            getHitsCount : function() {

                return $http.get(appConfig.asfAPIUrl + "/cachemanagement/gethitscount")
                    .success(function(data) {
                        return data;
                    });

            },
            generateCache : function(numberOfRecords) {

                return $http.get(appConfig.asfAPIUrl + "/cachemanagement/generate?numberOfRecords="+numberOfRecords)
                    .success(function(data) {
                        return data;
                    });

            },

            setCache : function(key, value) {

                return $http.get(appConfig.asfAPIUrl + "/beancache/set?key="+key+"&value="+value)
                    .success(function(data) {
                        return data;
                    });

            },

            removeCache : function(key) {

                return $http.get(appConfig.asfAPIUrl + "/beancache/remove?key="+key)
                    .success(function(data) {
                        return data;
                    });

            }


        };
    }

})();

