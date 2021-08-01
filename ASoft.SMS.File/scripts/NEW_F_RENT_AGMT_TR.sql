-- DROP TRIGGER NEW_F_RENT_AGMT_TR;

CREATE OR REPLACE TRIGGER NEW_F_RENT_AGMT_TR
    BEFORE INSERT ON F_RENT_AGMT FOR EACH ROW
BEGIN

    IF (:NEW.FRA_CONT_TYPE = 'RT') THEN
    
        INSERT INTO SMS_LOG (
            ID,
            TYPE,
            REF_ID,
            REF_TYPE,
            REF_LOC
        ) VALUES (
            SMS_LOG_SQ.NEXTVAL,
            'AGMT',
            :NEW.FRA_TRN_NO,
            :NEW.FRA_CONT_TYPE,
            :NEW.FRA_BRANCH_NO
        );
    
    END IF;
END NEW_F_RENT_AGMT_TR;
/

