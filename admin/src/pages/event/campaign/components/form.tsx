import { apiCampaignCreate, apiCampaignDetail, apiCampaignUpdate } from "@/services/event/campaign";
import { ModalForm, ModalFormProps, ProFormInstance, ProFormText } from "@ant-design/pro-components"
import { message } from "antd";
import { useEffect, useRef } from "react";

type Props = ModalFormProps & {
    data?: any;
    reload?: () => void;
}

const CampaignForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>(null);

    useEffect(() => {
        if (props.data && props.open) {
            apiCampaignDetail(props.data?.id).then((res) => {
                formRef.current?.setFields([
                    {
                        name: 'id',
                        value: res.data.id
                    },
                    {
                        name: 'code',
                        value: res.data.code
                    },
                    {
                        name: 'name',
                        value: res.data.name
                    }
                ])
            });
        }
    }, [props.data]);

    const onFinish = async (values: any) => {
        if (props.data) {
            await apiCampaignUpdate(values);
        } else {
            await apiCampaignCreate(values);
        }
        message.success("Thành công!");
        props.reload?.();
        formRef.current?.resetFields();
        return true;
    }

    return (
        <ModalForm {...props} title="Chiến dịch" formRef={formRef} onFinish={onFinish}>
            <ProFormText name={"id"} hidden />
            <ProFormText name={"code"} label="Mã chiến dịch" rules={[{ required: true }]} />
            <ProFormText name={"name"} label="Tên chiến dịch" rules={[{ required: true }]} />
        </ModalForm>
    )
}

export default CampaignForm;