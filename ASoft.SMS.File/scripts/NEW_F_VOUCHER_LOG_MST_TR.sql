-- DROP TRIGGER NEW_F_VOUCHER_LOG_MST_TR;

CREATE OR REPLACE TRIGGER NEW_F_VOUCHER_LOG_MST_TR
    BEFORE UPDATE ON F_VOUCHER_LOG_MST FOR EACH ROW
BEGIN

    IF (NEW.FVM_VOH_TYPE = 'TRV') THEN
    
        INSERT INTO SMS_LOG (
            ID,
            TYPE,
            REF_ID,
            REF_TYPE,
            REF_LOC
        ) VALUES (
            SMS_LOG_SQ.NEXTVAL,
            'TRV',
            NEW.FVM_VOH_NO,
            NEW.FVM_VOH_TYPE,
            NEW.FVM_LOC_NO
        );
    
    END IF;
END NEW_F_VOUCHER_LOG_MST_TR;


