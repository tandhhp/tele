import { apiCallOptions } from "@/services/call";
import { DrawerForm, DrawerFormProps, ProFormDatePicker, ProFormSelect, ProFormText, ProFormTimePicker } from "@ant-design/pro-components"
import { Col, Row } from "antd";

type Props = DrawerFormProps & {
    data?: any;
}

const CallForm: React.FC<Props> = (props) => {
    return (
        <DrawerForm {...props} title="Cuộc gọi">
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
            <ProFormText label="Khác" name="other" />
        </DrawerForm>
    )
}

export default CallForm;