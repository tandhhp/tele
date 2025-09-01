import { apiEventAdd, apiEventUpdate } from "@/services/event";
import { ModalForm, ModalFormProps, ProFormDatePicker, ProFormDateTimePicker, ProFormInstance, ProFormSelect, ProFormText, ProFormTimePicker } from "@ant-design/pro-components"
import { Col, message, Row } from "antd";
import { useRef } from "react";

type Props = ModalFormProps & {
    data?: any;
    reload?: () => void;
}

const EventForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>(null);

    const onFinish = async (values: any) => {
        if (props.data) {
            values.id = props.data.id;
            await apiEventUpdate(values);
        } else {
            await apiEventAdd(values);
        }
        message.success('Thành công!');
        formRef.current?.resetFields();
        props.reload?.();
        return true;
    }

    return (
        <ModalForm {...props} title="Sự kiện" formRef={formRef} onFinish={onFinish}>
            <ProFormText name={`name`} label="Tên sự kiện" rules={[
                {
                    required: true
                }
            ]} />
            <Row gutter={16}>
                <Col xs={12} md={6}>
                    <ProFormDatePicker name="startDate" width={`lg`} label="Ngày giờ diễn ra" rules={[{ required: true }]} />
                </Col>
                <Col xs={12} md={6}>
                    <ProFormTimePicker name="startTime" width={`lg`} label="Thời gian bắt đầu" rules={[{ required: true }]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name="status" label="Trạng thái" options={[
                        {
                            value: 0,
                            label: 'Lên kế hoạch'
                        },
                        {
                            value: 1,
                            label: 'Đang diễn ra'
                        },
                        {
                            value: 2,
                            label: 'Đã hoàn thành'
                        },
                        {
                            value: 3,
                            label: 'Đã hủy'
                        }
                    ]} rules={[{ required: true }]} />
                </Col>
            </Row>
        </ModalForm>
    )
}

export default EventForm;