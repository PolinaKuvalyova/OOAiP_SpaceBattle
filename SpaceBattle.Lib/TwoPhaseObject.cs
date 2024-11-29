using System.Collections.Generic;
using Hwdtech;
using System;

namespace SpaceBattle.Lib{

    public struct TwoPhaseValue{

        public TwoPhaseValue(object cur, object val, string id){
            this.currentValue = cur;
            this.transactionValue = val;
            this.transactionId = id;
        }

        public object currentValue { get; set; }
        public object transactionValue { get; set; }
        public string transactionId { get; set; }
    }

    public class TwoPhaseObject : IUObject{
        
        Dictionary<string, TwoPhaseValue> properties = new();

        public object get_property(string key){
            if (properties.ContainsKey(key)){
                if(properties[key].currentValue == properties[key].transactionValue){
                    return properties[key].currentValue;
                }
                else{
                    string transactionState = IoC.Resolve<string>("TransactionManager.GetByTransactionId", properties[key].transactionId);
                    if(transactionState == "Commited"){
                        properties[key] = new TwoPhaseValue(properties[key].transactionValue, properties[key].transactionValue, properties[key].transactionId);
                        return properties[key].currentValue;
                    }
                    else{
                        properties[key] = new TwoPhaseValue(properties[key].currentValue, properties[key].currentValue, properties[key].transactionId);
                        return properties[key].currentValue;
                    }
                }
            }   
            else{
                throw new Exception("Key not found in IUObject");
            }
        }

        public void set_property(string key, object value){
            if(properties.ContainsKey(key)){
                if(properties[key].currentValue == properties[key].transactionValue){
                    properties[key] = new TwoPhaseValue(properties[key].currentValue, value, IoC.Resolve<string>("CurrentTransaction.Id"));
                }
                else{
                    string transactionState = IoC.Resolve<string>("TransactionManager.GetByTransactionId", properties[key].transactionId);
                    if(transactionState == "Commited"){
                        properties[key] = new TwoPhaseValue(properties[key].transactionValue, value, IoC.Resolve<string>("CurrentTransaction.Id"));
                    }
                    else{
                        properties[key] = new TwoPhaseValue(properties[key].currentValue, value, IoC.Resolve<string>("CurrentTransaction.Id"));
                    }
                }
            }
            else{
                properties[key] = new TwoPhaseValue(null, value, IoC.Resolve<string>("CurrentTransaction.Id"));
            }
        }
    }
}
