import { apiCallComplete, apiCallOptions } from "@/services/call";
import { PhoneOutlined } from "@ant-design/icons";
import { DrawerForm, DrawerFormProps, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText, ProFormTextArea, ProFormTimePicker } from "@ant-design/pro-components"
import { Button, Col, message, Row } from "antd";
import { useRef } from "react";

type Props = DrawerFormProps & {
    data?: any;
    reload?: () => void;
}

const CallForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>(null);

    const onFinish = async (values: any) => {
        if (!props.data) {
            message.error('Liên hệ không tồn tại');
            return false;
        }
        values.contactId = props.data?.id;
        await apiCallComplete(values);
        message.success('Lưu thành công');
        formRef.current?.resetFields();
        props.reload?.();
        return true;
    }

    return (
        <DrawerForm {...props} title={`Cuộc gọi ${props.data?.name}`} onFinish={onFinish} formRef={formRef}>
            <Button type="primary" icon={<PhoneOutlined />} block href={`tel:${props.data?.phoneNumber}`} className="mb-4">Gọi điện</Button>
            <Row gutter={[16, 16]}>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`callStatusId`} label="Trạng thái" request={apiCallOptions} showSearch
                        rules={[
                            {
                                required: true
                            }
                        ]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`extraStatus`} label="Trạng thái bổ sung" options={[
                        {
                            label: 'Có tiền',
                            value: 'Có tiền'
                        },
                        {
                            label: 'Có thói quen',
                            value: 'Có thói quen'
                        },
                        {
                            label: 'Có cả 2',
                            value: 'Có cả 2'
                        }
                    ]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormDatePicker name="followUpDate" label="Ngày theo dõi" width="lg" />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormTimePicker name="followUpTime" label="Giờ theo dõi" width="lg" />
                </Col>
            </Row>
            <ProFormText label="Thói quen du lịch" name="travelHabit" />
            <ProFormText label="Tuổi" name="age" />
            <ProFormText label="Nghề nghiệp" name="job" />
            <ProFormTextArea label="Ghi chú" name="note" />
        </DrawerForm>
    )
}

export default CallForm;