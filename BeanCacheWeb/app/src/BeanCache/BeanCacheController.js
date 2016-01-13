/**
 * Created by prveerat on 10/2/2015.
 */
(function(){

    angular
        .module('beanCache')
        .controller('BeanCacheController', [
            'beanCacheService', '$mdSidenav', '$mdBottomSheet', '$log', '$q', '$scope', '$mdDialog', 'appConfig',
            BeanCacheController
        ]);


    function BeanCacheController( beanCacheService, $mdSidenav, $mdBottomSheet, $log, $q, $scope, $mdDialog, appConfig) {
        $scope.partitionData = null;
        $scope.chartObject = {};
        $scope.partitionChartType = "ColumnChart";
        
        $scope.selectedCMItem = null;
        $scope.cacheData = null;
        $scope.generateCacheSuccess = false;
        $scope.numberOfRecords = 0;
        $scope.generateCacheSubmit = false;
        $scope.showCacheSubmit = false;
        $scope.setCacheDurationStart = null;
        $scope.setCacheDurationEnd = null;
        $scope.setCacheDuration = null;

        $scope.cacheItem = {
            key: '',
            value: ''
        };


        $scope.getHitsCount = function()
        {
            beanCacheService
                .getHitsCount()
                .then( function( data ) {
                    $scope.partitionData = data.data;
                    $log.info($scope.partitionData);
                    if($scope.partitionChartType === null)
                    {
                        $scope.partitionChartType = "ColumnChart";
                    }
                    if($scope.partitionData !=null &&$scope.partitionData.CacheInfo != null)
                        {
                            $scope.chartObject.type = $scope.partitionChartType;
                            $scope.chartObject.data = {"cols": [
                                {id: "p", label: "Partition", type: "string"},
                                {id: "k", label: "Keys", type: "number"}
                            ], "rows": [
                            {c: [
                                {v: $scope.partitionData.CacheInfo[0].LowKey + "-" + $scope.partitionData.CacheInfo[0].HighKey},
                                {v: $scope.partitionData.CacheInfo[0].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[1].LowKey + "-" + $scope.partitionData.CacheInfo[1].HighKey},
                                {v: $scope.partitionData.CacheInfo[1].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[2].LowKey + "-" + $scope.partitionData.CacheInfo[2].HighKey},
                                {v: $scope.partitionData.CacheInfo[2].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[3].LowKey + "-" + $scope.partitionData.CacheInfo[3].HighKey},
                                {v: $scope.partitionData.CacheInfo[3].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[4].LowKey + "-" + $scope.partitionData.CacheInfo[4].HighKey},
                                {v: $scope.partitionData.CacheInfo[4].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[5].LowKey + "-" + $scope.partitionData.CacheInfo[5].HighKey},
                                {v: $scope.partitionData.CacheInfo[5].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[6].LowKey + "-" + $scope.partitionData.CacheInfo[6].HighKey},
                                {v: $scope.partitionData.CacheInfo[6].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[7].LowKey + "-" + $scope.partitionData.CacheInfo[7].HighKey},
                                {v: $scope.partitionData.CacheInfo[7].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[8].LowKey + "-" + $scope.partitionData.CacheInfo[8].HighKey},
                                {v: $scope.partitionData.CacheInfo[8].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[9].LowKey + "-" + $scope.partitionData.CacheInfo[9].HighKey},
                                {v: $scope.partitionData.CacheInfo[9].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[10].LowKey + "-" + $scope.partitionData.CacheInfo[10].HighKey},
                                {v: $scope.partitionData.CacheInfo[10].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[11].LowKey + "-" + $scope.partitionData.CacheInfo[11].HighKey},
                                {v: $scope.partitionData.CacheInfo[11].Hits},
                            ]},
                            {c: [
                                {v: $scope.partitionData.CacheInfo[12].LowKey + "-" + $scope.partitionData.CacheInfo[12].HighKey},
                                {v: $scope.partitionData.CacheInfo[12].Hits},
                            ]},

                        ]};

                            $scope.chartObject.options = {
                                'title': 'Keys Per Partition',
                                'logScale': true
                            };

                    }

                });

        };

        $scope.partitionChartTypeChange = function(item) {
            $scope.partitionChartType = item;
            $scope.getHitsCount();
        };

        //Force load chart first time
        $scope.partitionChartType = "ColumnChart";
        $scope.getHitsCount();
        

        $scope.showCache = function(numberOfRecords)
        {
            $scope.showCacheSubmit = true;
            $scope.showCacheDurationStart = null;
            $scope.showCacheDurationEnd = null;
            $scope.showCacheDuration = null;
            $scope.showCacheDurationPerRecord = null;
            $scope.showCacheDurationStart = performance.now();
            beanCacheService
                .getAllCache(numberOfRecords)
                .then( function( data ) {
                    $scope.showCacheDurationEnd = performance.now();
                    $scope.showCacheDuration = ($scope.showCacheDurationEnd-$scope.showCacheDurationStart);
                    $scope.showCacheDurationPerRecord = $scope.showCacheDuration/numberOfRecords;
                    $scope.cacheData = data;
                });
        };

        $scope.generateCache = function(numberOfRecords, ev)
        {
            $scope.generateCacheSubmit = true;
            $scope.generateCacheDurationStart = null;
            $scope.generateCacheDurationEnd = null;
            $scope.generateCacheDuration = null;
            $scope.generateCacheDurationPerRecord = null;
            $scope.generateCacheDurationStart = performance.now();
            beanCacheService
                .generateCache(numberOfRecords)
                .then( function( data ) {
                    $scope.generateCacheDurationEnd = performance.now();
                    $scope.generateCacheDuration = ($scope.generateCacheDurationEnd-$scope.generateCacheDurationStart);
                    $scope.generateCacheDurationPerRecord = $scope.generateCacheDuration/numberOfRecords;
                    $scope.generateCacheSuccess = true;

                    $mdDialog.show(
                        $mdDialog.alert().parent(angular.element(document.querySelector('#beanCacheContainer')))
                            .clickOutsideToClose(true)
                            .title('Confirmation')
                            .content('Cache generated successfully!')
                            .ariaLabel('Alert')
                            .ok('OK')
                            .targetEvent(ev)
                    );
                });
        };

        $scope.setCache = function()
        {
            beanCacheService
                .setCache($scope.cacheItem.key, $scope.cacheItem.value)
                .then(onSuccess, onError);
        };

        $scope.getCache = function () {
            beanCacheService
                .getCache($scope.cacheItem.key)
                .then(onSuccessGet, onError);

        };

        $scope.removeCache = function () {
            $scope.callDuration = null;

            beanCacheService
                .removeCache($scope.cacheItem.key)
                .then(onSuccess, onError);
        };

        onSuccess = function (response)
        {
            $scope.callDuration = response.data;
            $scope.message = "Success!!";
        }

        onSuccessGet = function (response) {
            $scope.callDuration = response.data.TimeMilliseconds;
            $scope.cacheItem.value = response.data.Value;
            $scope.message = "Success!!";
        }

        onError = function (reason) {
            $scope.callDuration = -1;
            $scope.message = "Failed!!  " + reason.statusText;
        }

        


        $scope.toggleCM = function(item, ev){

            if(item==='generate')
            {
                $scope.selectedCMItem = 'generate';
                $scope.generateCacheSubmit = false;
            }

            if(item==='show')
            {
                $scope.selectedCMItem = 'show';
                $scope.showCacheSubmit = false;

            }

            if(item==='set')
            {
                $scope.cacheItem = {
                    key: '',
                    value: ''
                };
                $scope.selectedCMItem = 'set';

            }

            if(item==='query')
            {
                $scope.cacheItem = {
                    key: '',
                    value: ''
                };
                $scope.selectedCMItem = 'query';
            }

            if(item==='remove')
            {
                $scope.cacheItem = {
                    key: '',
                    value: ''
                };
                $scope.selectedCMItem = 'remove';


            }
        };

        $scope.setASFURL = function(ev)
        {
            appConfig.asfAPIUrl = $scope.asfURL;
        };

        $scope.asfURL = appConfig.asfAPIUrl;
 }



})();
